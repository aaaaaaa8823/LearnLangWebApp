let currentLesson = null;
let lessonType = null;

async function loadLesson() {
    const container = document.getElementById('lessonContainer');

    const urlParams = new URLSearchParams(window.location.search);
    const lessonId = urlParams.get('id');
    lessonType = urlParams.get('type');

    if (!lessonId) {
        container.innerHTML = '<div class="error">ID урока не указан</div>';
        console.log("ID урока не указан");
        return;
    }

    if (!lessonType) {
        lessonType = 'grammar'; 
    }

    try {
        const token = localStorage.getItem('token');
        if (!token) {
            window.location.href = '/';
            return;
        }

        const apiUrl = lessonType === 'grammar'
            ? `/api/Grammar/topic/${lessonId}`
            : `/api/Vocab/topic/${lessonId}`;

        const response = await fetch(apiUrl, {
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (!response.ok) {
            if (response.status === 404) {
                throw new Error('Урок не найден');
            }
            throw new Error('Ошибка загрузки урока');
        }

        currentLesson = await response.json();
        console.log('Урок загружен:', currentLesson);

        displayLesson();

    } catch (error) {
        console.error('Ошибка:', error);
        container.innerHTML = `<div class="error">${error.message}</div>`;
    }
}

function displayLesson() {
    const container = document.getElementById('lessonContainer');

    if (!currentLesson) {
        container.innerHTML = '<div class="error">Данные урока не загружены</div>';
        return;
    }

    const isCompleted = currentLesson.userProgress?.completed || false;
    const completedAt = currentLesson.userProgress?.completedAt;
    const completedDate = completedAt ? new Date(completedAt).toLocaleDateString('ru-RU') : '';

    const typeLabel = lessonType === 'vocab' ? 'Вокабуляр' : 'Грамматика';

    const theoryContent = getTheoryContent(currentLesson.title);
    const examplesContent = getExamplesContent(currentLesson.title);

    container.innerHTML = `
        <div class="lesson-container">
            <div class="lesson-header">
                <div class="lesson-level">Уровень: ${escapeHtml(currentLesson.level)} | Тип: ${typeLabel}</div>
            </div>
            
            <div class="lesson-body">
                <div class="theory-section">
                    <h2>Теория</h2>
                    <div class="theory-content">
                        ${theoryContent}
                    </div>
                </div>
                
                <div class="examples-section">
                    <h2>Примеры</h2>
                    <ul class="examples-list">
                        ${examplesContent.map(ex => `<li>${escapeHtml(ex)}</li>`).join('')}
                    </ul>
                </div>
            </div>
            
            <div class="lesson-footer">
                ${!isCompleted ? `
                    <button class="btn-complete" onclick="markLessonComplete()">
                        Отметить как пройденный
                    </button>
                ` : `
                    <div class="completed-message">
                        ✓ Урок пройден ${completedDate}
                    </div>
                `}
                
                ${currentLesson.tests && currentLesson.tests.length > 0 ? `
                    <div class="available-tests">
                        <h3>Доступные тесты:</h3>
                        <div class="tests-list">
                            ${currentLesson.tests.map(test => `
                                <div class="test-card">
                                    <div class="test-info">
                                        <span class="test-title">${escapeHtml(test.title)}</span>
                                        <span class="test-details">
                                            ${test.questionCount} вопросов | 
                                            Проходной балл: ${test.passingScore}% | 
                                            Время: ${test.timeLimitMinutes} мин
                                        </span>
                                    </div>
                                    <button class="btn-test" onclick="startTest(${test.id}, '${lessonType}')">
                                        Пройти тест
                                    </button>
                                </div>
                            `).join('')}
                        </div>
                    </div>
                ` : `
                    <div class="available-tests">
                        <p style="color: #6b7a8f; text-align: center;">К этому уроку пока нет тестов</p>
                    </div>
                `}
            </div>
        </div>
    `;
}

async function markLessonComplete() {
    if (!currentLesson) return;

    try {
        const token = localStorage.getItem('token');

        const apiUrl = lessonType === 'grammar'
            ? `/api/Grammar/topic/${currentLesson.id}/complete`
            : `/api/Vocab/topic/${currentLesson.id}/complete`;

        const response = await fetch(apiUrl, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Ошибка');
        }

        alert('Урок отмечен как пройденный!');

        window.location.reload();

    } catch (error) {
        console.error('Ошибка:', error);
        alert(error.message);
    }
}

function startTest(testId, type) {
    window.location.href = `/Home/TestDetail?id=${testId}&type=${type}`;
}

function getExamplesContent(title) {
    const examplesMap = {
        'Present Simple': [
            'I work every day. (Я работаю каждый день)',
            'She works in an office. (Она работает в офисе)',
            'Water boils at 100 degrees. (Вода кипит при 100 градусах)'
        ],
        'Past Simple': [
            'I worked yesterday. (Я работал вчера)',
            'She went to London last year. (Она ездила в Лондон в прошлом году)'
        ],
        'Future Simple': [
            'I will call you tomorrow. (Я позвоню тебе завтра)',
            'She will be here soon. (Она скоро будет здесь)'
        ],
        'My Daily Routine': [
            'I wake up at 7 AM every day. (Я просыпаюсь в 7 утра каждый день)',
            'I have breakfast at 8 AM. (Я завтракаю в 8 утра)',
            'I go to work at 9 AM. (Я иду на работу в 9 утра)'
        ],
        'My Family': [
            'I have a big family. (У меня большая семья)',
            'My mother is a doctor. (Моя мама врач)',
            'My father works in an office. (Мой папа работает в офисе)'
        ]
    };

    return examplesMap[title] || [
        'Пример 1 с переводом',
        'Пример 2 с переводом',
        'Пример 3 с переводом'
    ];
}

function getTheoryContent(title) {
    const contentMap = {
        'Present Simple': `
            <p><strong>Present Simple (настоящее простое время)</strong> используется для обозначения:</p>
            <ul>
                <li>Обычных, повторяющихся действий</li>
                <li>Фактов и общих истин</li>
                <li>Расписаний и графиков</li>
            </ul>
            <h3>Образование утвердительных предложений:</h3>
            <p>I/You/We/They + глагол (без окончания)<br>
            He/She/It + глагол + s/es</p>
        `,
        'Past Simple': `
            <p><strong>Past Simple (прошедшее простое время)</strong> используется для обозначения действий, которые произошли в прошлом.</p>
            <h3>Правильные глаголы:</h3>
            <p>глагол + ed (work → worked)</p>
        `,
        'Future Simple': `
            <p><strong>Future Simple (будущее простое время)</strong> используется для обозначения действий, которые произойдут в будущем.</p>
            <h3>Образование:</h3>
            <p>will + глагол (без частицы to)</p>
        `,
        'My Daily Routine': `
            <p><strong>Daily Routine (повседневные дела)</strong> - это действия, которые мы выполняем каждый день.</p>
            <h3>Основные глаголы:</h3>
            <ul>
                <li>wake up - просыпаться</li>
                <li>get dressed - одеваться</li>
                <li>have breakfast - завтракать</li>
                <li>go to work/school - идти на работу/в школу</li>
                <li>have lunch - обедать</li>
                <li>come home - возвращаться домой</li>
                <li>have dinner - ужинать</li>
                <li>go to bed - ложиться спать</li>
            </ul>
        `,
        'My Family': `
            <p><strong>Family (семья)</strong> - это самые близкие люди.</p>
            <h3>Члены семьи:</h3>
            <ul>
                <li>mother/mom - мама</li>
                <li>father/dad - папа</li>
                <li>brother - брат</li>
                <li>sister - сестра</li>
                <li>grandmother - бабушка</li>
                <li>grandfather - дедушка</li>
                <li>aunt - тётя</li>
                <li>uncle - дядя</li>
                <li>cousin - двоюродный брат/сестра</li>
            </ul>
        `
    };

    return contentMap[title] || `
        <p>Содержание урока "${title}" будет добавлено позже.</p>
        <p>Здесь будет представлена теория по данной теме с примерами и объяснениями.</p>
    `;
}

function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

document.addEventListener('DOMContentLoaded', loadLesson);