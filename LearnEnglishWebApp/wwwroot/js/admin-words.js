let examples = [];

async function loadWords() {
    const container = document.getElementById('wordsList');

    try {
        const response = await fetch('/api/admin/words', {
            credentials: 'include'
        });

        if (!response.ok) throw new Error('Ошибка загрузки');

        const words = await response.json();
        displayWords(words);

        document.getElementById('wordsCount').textContent = words.length;

    } catch (error) {
        console.error('Ошибка:', error);
        container.innerHTML = '<div class="empty">Ошибка загрузки слов</div>';
    }
}

function displayWords(words) {
    const container = document.getElementById('wordsList');

    if (!words || words.length === 0) {
        container.innerHTML = '<div class="empty">Слова не найдены</div>';
        return;
    }

    container.innerHTML = `
        <table class="data-table">
            <thead>
                <tr><th>Слово</th><th>Перевод</th><th>Часть речи</th><th>Уровень</th><th>Действия</th></tr>
            </thead>
            <tbody>
                ${words.map(word => `
                    <tr>
                        <td>${escapeHtml(word.word)}</td>
                        <td>${escapeHtml(word.translation)}</td>
                        <td>${escapeHtml(word.partOfSpeech || '-')}</td>
                        <td>${escapeHtml(word.difficultyLevel || '-')}</td>
                        <td>
                            <button class="btn-edit" onclick="editWord(${word.id})">Редактировать</button>
                            <button class="btn-delete" onclick="deleteWord(${word.id})">Удалить</button>
                        </td>
                    </tr>
                `).join('')}
            </tbody>
        </table>
    `;
}

function addExample() {
    const input = document.getElementById('exampleInput');
    const example = input.value.trim();

    if (example) {
        examples.push(example);
        updateExamplesList();
        input.value = '';
    }
}

function updateExamplesList() {
    const container = document.getElementById('examplesList');
    container.innerHTML = examples.map((ex, idx) => `
        <span class="example-tag">
            ${escapeHtml(ex)}
            <span class="remove" onclick="removeExample(${idx})">
                <i class="fas fa-xmark"></i>
            </span>
        </span>
    `).join('');
}

function removeExample(index) {
    examples.splice(index, 1);
    updateExamplesList();
}

document.getElementById('wordForm').addEventListener('submit', async (e) => {
    e.preventDefault();

    const id = document.getElementById('wordId').value;
    const word = document.getElementById('word').value.trim();
    const translation = document.getElementById('translation').value.trim();
    const partOfSpeech = document.getElementById('partOfSpeech').value;
    const difficultyLevel = document.getElementById('difficultyLevel').value;
    const definition = document.getElementById('definition').value.trim();

    if (!word || !translation) {
        showMessage('Заполните обязательные поля', 'error');
        return;
    }

    const data = {
        word, translation, partOfSpeech, difficultyLevel, definition,
        examples: examples
    };

    try {
        const url = id && id !== '0'
            ? `/api/admin/words/${id}`
            : '/api/admin/words';

        const method = id && id !== '0' ? 'PUT' : 'POST';

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

        showMessage('Слово успешно сохранено!', 'success');
        resetForm();
        loadWords();

    } catch (error) {
        showMessage(error.message, 'error');
    }
});

async function editWord(id) {
    try {
        const response = await fetch(`/api/admin/words/${id}`, {
            credentials: 'include'
        });

        if (!response.ok) throw new Error('Ошибка загрузки');

        const word = await response.json();

        document.getElementById('wordId').value = word.id;
        document.getElementById('word').value = word.word;
        document.getElementById('translation').value = word.translation;
        document.getElementById('partOfSpeech').value = word.partOfSpeech || '';
        document.getElementById('difficultyLevel').value = word.difficultyLevel || 'A1';
        document.getElementById('definition').value = word.definition || '';

        examples = word.examples || [];
        updateExamplesList();

        document.getElementById('formTitle').textContent = 'Редактировать слово';

        document.querySelector('.form-section').scrollIntoView({ behavior: 'smooth' });

    } catch (error) {
        showMessage(error.message, 'error');
    }
}

async function deleteWord(id) {
    if (!confirm('Вы уверены, что хотите удалить это слово?')) return;

    try {
        const response = await fetch(`/api/admin/words/${id}`, {
            method: 'DELETE',
            credentials: 'include'
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Ошибка удаления');
        }

        showMessage('Слово удалено', 'success');
        loadWords();

    } catch (error) {
        showMessage(error.message, 'error');
    }
}

function resetForm() {
    document.getElementById('wordId').value = '0';
    document.getElementById('word').value = '';
    document.getElementById('translation').value = '';
    document.getElementById('partOfSpeech').value = '';
    document.getElementById('difficultyLevel').value = 'A1';
    document.getElementById('definition').value = '';
    examples = [];
    updateExamplesList();
    document.getElementById('formTitle').textContent = 'Добавить новое слово';
}

function showMessage(text, type) {
    const container = document.getElementById('messageArea');
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
    loadWords();
});