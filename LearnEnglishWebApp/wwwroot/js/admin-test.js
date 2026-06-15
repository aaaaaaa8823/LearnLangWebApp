let allTopics = [];
let questions = [];

function addQuestion() {
    const questionCount = parseInt(document.getElementById('questionCount').value) || 0;

    if (questions.length >= questionCount) {
        showMessage(`Нельзя добавить больше ${questionCount} вопросов`, 'error');
        return;
    }

    const index = questions.length;

    questions.push({
        id: index,
        text: '',
        options: ['', '', '', ''],
        correctOption: 0
    });

    renderQuestions();
}
function removeQuestion(index) {
    questions.splice(index, 1);
    renderQuestions();
}

function updateQuestionText(index, value) {
    questions[index].text = value;
}

function updateOption(questionIndex, optionIndex, value) {
    questions[questionIndex].options[optionIndex] = value;
}

function updateCorrectOption(questionIndex, value) {
    questions[questionIndex].correctOption = parseInt(value);
}

function renderQuestions() {
    const container = document.getElementById('questionsContainer');

    if (questions.length === 0) {
        container.innerHTML = '<div class="empty-questions">Нажмите "Добавить вопрос" чтобы начать</div>';
        return;
    }

    container.innerHTML = questions.map((q, idx) => `
        <div class="question-card">
            <div class="question-header">
                <span class="question-number">Вопрос ${idx + 1}</span>
            </div>
            <div class="question-text-input">
                <label>Текст вопроса:</label>
                <input type="text" value="${escapeHtml(q.text)}" 
                       onchange="updateQuestionText(${idx}, this.value)" 
                       placeholder="Например: I ___ to school every day.">
            </div>
            <div class="options-group">
                <label>Варианты ответов:</label>
                <div class="options-list-editor">
                    ${q.options.map((opt, optIdx) => `
                        <div class="option-row">
                            <input type="text" class="option-input" 
                                   value="${escapeHtml(opt)}" 
                                   placeholder="Вариант ${optIdx + 1}"
                                   onchange="updateOption(${idx}, ${optIdx}, this.value)">
                            <div class="option-correct">
                                <input type="radio" name="correct_${idx}" value="${optIdx}"
                                       ${q.correctOption === optIdx ? 'checked' : ''}
                                       onchange="updateCorrectOption(${idx}, this.value)">
                                <span>Правильный</span>
                            </div>
                        </div>
                    `).join('')}
                </div>
            </div>
        </div>
    `).join('');
}

document.getElementById('questionCount').addEventListener('change', () => {
    const newCount = parseInt(document.getElementById('questionCount').value) || 0;
    const currentCount = questions.length;

    if (newCount > currentCount) {
        for (let i = currentCount; i < newCount; i++) {
            questions.push({
                id: i,
                text: '',
                options: ['', '', '', ''],
                correctOption: 0
            });
        }
        renderQuestions();
        showMessage(`Добавлено ${newCount - currentCount} пустых вопросов. Заполните их.`, 'success');
    }
    else if (newCount < currentCount) {
        if (confirm(`Вы действительно хотите удалить ${currentCount - newCount} последних вопросов?`)) {
            questions = questions.slice(0, newCount);
            renderQuestions();
            showMessage(`Удалено ${currentCount - newCount} вопросов`, 'success');
        } else {
            document.getElementById('questionCount').value = currentCount;
        }
    }
});

async function loadTopics() {
    try {
        const response = await fetch('/api/admin/grammar/topics', {
            credentials: 'include'
        });

        if (!response.ok) throw new Error('Ошибка загрузки уроков');

        allTopics = await response.json();
        const select = document.getElementById('topicId');

        select.innerHTML = '<option value="">Выберите урок</option>' +
            allTopics.map(topic => `<option value="${topic.id}">${escapeHtml(topic.title)} (${topic.level})</option>`).join('');

    } catch (error) {
        console.error('Ошибка загрузки уроков:', error);
    }
}

async function loadTests() {
    const container = document.getElementById('testsList');
    if (!container) return;

    try {
        const response = await fetch('/api/admin/grammar/tests', {
            credentials: 'include'
        });

        if (!response.ok) throw new Error('Ошибка загрузки тестов');

        const tests = await response.json();
        displayTests(tests);

        const countEl = document.getElementById('testsCount');
        if (countEl) countEl.textContent = tests.length;

    } catch (error) {
        console.error('Ошибка:', error);
        container.innerHTML = '<div class="empty">Ошибка загрузки тестов</div>';
    }
}

function displayTests(tests) {
    const container = document.getElementById('testsList');

    if (!tests || tests.length === 0) {
        container.innerHTML = '<div class="empty">Тесты не найдены</div>';
        return;
    }

    container.innerHTML = `
        <table class="data-table">
            <thead>
                <tr><th>Название</th><th>Урок</th><th>Уровень</th><th>Вопросов</th><th>Балл</th><th>Время</th><th>Действия</th></tr>
            </thead>
            <tbody>
                ${tests.map(test => `
                    <tr>
                        <td><strong>${escapeHtml(test.title)}</strong></td>
                        <td>${escapeHtml(test.grammarTopic?.title || '-')}</td>
                        <td>${escapeHtml(test.level)}</td>
                        <td>${test.questionCount}</td>
                        <td>${test.passingScore}%</td>
                        <td>${test.timeLimitMinutes} мин</td>
                        <td>
                            <button class="btn-edit" onclick="editTest(${test.id})">Редактировать</button>
                            <button class="btn-delete" onclick="deleteTest(${test.id})">Удалить</button>
                        </td>
                    </tr>
                `).join('')}
            </tbody>
        </table>
    `;
}

async function editTest(id) {
    try {
        const response = await fetch(`/api/admin/grammar/tests/${id}`, {
            credentials: 'include'
        });

        if (!response.ok) throw new Error('Ошибка загрузки');

        const test = await response.json();

        document.getElementById('testId').value = test.id;
        document.getElementById('topicId').value = test.grammarTopicId;
        document.getElementById('level').value = test.level;
        document.getElementById('title').value = test.title;
        document.getElementById('orderIndex').value = test.orderIndex || 0;
        document.getElementById('questionCount').value = test.questionCount;
        document.getElementById('passingScore').value = test.passingScore;
        document.getElementById('timeLimitMinutes').value = test.timeLimitMinutes;

        parseQuestionsFromText(test.questionsText, test.answersText);
        renderQuestions();

        document.getElementById('formTitle').textContent = 'Редактировать тест';

        document.querySelector('.form-section').scrollIntoView({ behavior: 'smooth' });

    } catch (error) {
        showMessage(error.message, 'error');
    }
}

function parseQuestionsFromText(questionsText, answersText) {
    questions = [];

    if (!questionsText) return;

    const questionLines = questionsText.split('\n').filter(l => l.trim());
    const answerLines = answersText?.split('\n').filter(l => l.trim()) || [];

    for (let i = 0; i < questionLines.length; i++) {
        const line = questionLines[i];
        const parts = line.split('|').map(p => p.trim());

        const questionText = parts[0];
        const options = parts.slice(1, 5);

        while (options.length < 4) options.push('');

        let correctOption = 0;
        if (answerLines[i]) {
            const correctAnswer = answerLines[i].trim();
            correctOption = options.findIndex(opt => opt === correctAnswer);
            if (correctOption === -1) correctOption = 0;
        }

        questions.push({
            id: i,
            text: questionText,
            options: options,
            correctOption: correctOption
        });
    }
}

function formatQuestionsToText() {
    const questionsText = questions.map(q => {
        const options = q.options.filter(opt => opt).join(' | ');
        return `${q.text}${options ? ' | ' + options : ''}`;
    }).join('\n');

    const answersText = questions.map(q => q.options[q.correctOption]).join('\n');

    return { questionsText, answersText };
}

document.getElementById('testForm').addEventListener('submit', async (e) => {
    e.preventDefault();

    const id = document.getElementById('testId').value || '0';
    const grammarTopicId = document.getElementById('topicId').value;
    const level = document.getElementById('level').value;
    const title = document.getElementById('title').value.trim();
    const orderIndex = parseInt(document.getElementById('orderIndex').value) || 0;
    const questionCount = parseInt(document.getElementById('questionCount').value) || questions.length;
    const passingScore = parseInt(document.getElementById('passingScore').value) || 0;
    const timeLimitMinutes = parseInt(document.getElementById('timeLimitMinutes').value) || 0;

    if (!grammarTopicId) {
        showMessage('Выберите грамматический урок', 'error');
        return;
    }

    if (!title) {
        showMessage('Введите название теста', 'error');
        return;
    }

    if (questions.length === 0) {
        showMessage('Добавьте хотя бы один вопрос', 'error');
        return;
    }

    if (questions.length !== questionCount) {
        showMessage(`Количество вопросов (${questions.length}) не соответствует указанному (${questionCount}). Добавьте или удалите вопросы.`, 'error');
        return;
    }

    const emptyQuestions = questions.some(q => !q.text.trim());
    if (emptyQuestions) {
        showMessage('Заполните текст всех вопросов', 'error');
        return;
    }

    let hasEmptyOption = false;
    for (let i = 0; i < questions.length; i++) {
        const emptyOption = questions[i].options.some(opt => !opt.trim());
        if (emptyOption) {
            showMessage(`В вопросе ${i + 1} не заполнены все варианты ответов`, 'error');
            return;
        }
    }

    const { questionsText, answersText } = formatQuestionsToText();

    const data = {
        grammarTopicId: parseInt(grammarTopicId),
        level: level,
        title: title,
        orderIndex: orderIndex,
        questionCount: questions.length,
        passingScore: passingScore,
        timeLimitMinutes: timeLimitMinutes,
        questionsText: questionsText,
        answersText: answersText
    };

    try {
        const url = id !== '0' ? `/api/admin/grammar/tests/${id}` : '/api/admin/grammar/tests';
        const method = id !== '0' ? 'PUT' : 'POST';

        const response = await fetch(url, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data),
            credentials: 'include'
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Ошибка сохранения');
        }

        showMessage('Тест успешно сохранён!', 'success');
        resetForm();
        loadTests();

    } catch (error) {
        console.error('Ошибка:', error);
        showMessage(error.message, 'error');
    }
});

async function deleteTest(id) {
    if (!confirm('Вы уверены, что хотите удалить этот тест?')) return;

    try {
        const response = await fetch(`/api/admin/grammar/tests/${id}`, {
            method: 'DELETE',
            credentials: 'include'
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Ошибка удаления');
        }

        showMessage('Тест удалён', 'success');
        loadTests();

    } catch (error) {
        showMessage(error.message, 'error');
    }
}

function resetForm() {
    document.getElementById('testId').value = '0';
    document.getElementById('topicId').value = '';
    document.getElementById('level').value = 'A1';
    document.getElementById('title').value = '';
    document.getElementById('orderIndex').value = '0';
    document.getElementById('questionCount').value = '1';
    document.getElementById('passingScore').value = '70';
    document.getElementById('timeLimitMinutes').value = '10';

    questions = [];
    renderQuestions();
    addQuestion();
    document.getElementById('formTitle').textContent = 'Добавить новый тест';
}

function showMessage(text, type) {
    const container = document.getElementById('messageArea');
    if (!container) return;

    container.innerHTML = `<div class="message ${type}">${escapeHtml(text)}</div>`;
    setTimeout(() => {
        container.innerHTML = '';
    }, 3000);
}

function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

document.addEventListener('DOMContentLoaded', () => {
    loadTopics();
    loadTests();
    addQuestion();
});