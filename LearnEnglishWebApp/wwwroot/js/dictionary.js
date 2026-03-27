let allWords = [];
let learningWords = 0;
let learnedWords = 0;

async function loadUserWordStats() {
    try {
        const token = localStorage.getItem('token');
        if (!token) {
            window.location.href = '/';
            return;
        }

        const response = await fetch('/api/Profile/stats', {
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (response.ok) {
            const stats = await response.json();
            learningWords = stats.learningWords || 0;
            learnedWords = stats.learnedWords || 0;

            const learningElement = document.getElementById('learningWords');
            const learnedElement = document.getElementById('learnedWords');

            if (learningElement) learningElement.textContent = learningWords;
            if (learnedElement) learnedElement.textContent = learnedWords;
        }
    } catch (error) {
        console.error('Ошибка загрузки статистики слов:', error);
    }
}

async function loadDictionaryWords() {
    const container = document.getElementById('wordsList');
    if (!container) return;

    try {
        const token = localStorage.getItem('token');
        const response = await fetch('/api/Dictionary/all', {
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (!response.ok) throw new Error('Ошибка загрузки слов');

        allWords = await response.json();
        displayWords(allWords);

    } catch (error) {
        console.error('Ошибка:', error);
        container.innerHTML = '<div class="error">Ошибка загрузки слов</div>';
    }
}

function displayWords(words) {
    const container = document.getElementById('wordsList');

    if (!words || words.length === 0) {
        container.innerHTML = '<div class="empty">Слова не найдены</div>';
        return;
    }

    container.innerHTML = words.map(word => `
        <div class="word-item" data-word-id="${word.id}">
            <div class="word-info">
                <span class="word-text">${escapeHtml(word.word)}</span>
                <span class="word-translation">${escapeHtml(word.translation)}</span>
                ${word.partOfSpeech ? `<span class="word-part">${escapeHtml(word.partOfSpeech)}</span>` : ''}
                ${word.difficultyLevel ? `<span class="word-level">${escapeHtml(word.difficultyLevel)}</span>` : ''}
            </div>
            <div class="word-actions">
                <button class="btn-add" onclick="addWordToLearning(${word.id}, '${escapeHtml(word.word)}')">
                    + Добавить
                </button>
            </div>
            ${word.examples && word.examples.length > 0 ? `
                <div class="word-examples">
                    ${word.examples.map(ex => `<div class="example">${escapeHtml(ex)}</div>`).join('')}
                </div>
            ` : ''}
        </div>
    `).join('');
}

async function searchWords() {
    const searchTerm = document.getElementById('wordSearch').value;

    if (!searchTerm || searchTerm.trim() === '') {
        displayWords(allWords);
        return;
    }

    try {
        const token = localStorage.getItem('token');
        const response = await fetch(`/api/Dictionary/search?q=${encodeURIComponent(searchTerm)}`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (!response.ok) throw new Error('Ошибка поиска');

        const filteredWords = await response.json();
        displayWords(filteredWords);

    } catch (error) {
        console.error('Ошибка поиска:', error);
    }
}

async function addWordToLearning(wordId, wordText) {
    try {
        const token = localStorage.getItem('token');
        const user = JSON.parse(localStorage.getItem('user'));

        const response = await fetch('/api/UserWords/add', {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                userId: user.id,
                wordId: wordId,
                status: 'learning'
            })
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Ошибка добавления слова');
        }

        learningWords++;
        const learningElement = document.getElementById('learningWords');
        if (learningElement) learningElement.textContent = learningWords;

        alert(`Слово "${wordText}" добавлено в список "В процессе"`);

        const button = event.target;
        button.textContent = '✓ Добавлено';
        button.disabled = true;

    } catch (error) {
        console.error('Ошибка:', error);
        alert(error.message);
    }
}

document.addEventListener('DOMContentLoaded', async () => {
    await loadUserWordStats();
    await loadDictionaryWords();

    const searchInput = document.getElementById('wordSearch');
    if (searchInput) {
        let timeout;
        searchInput.addEventListener('input', () => {
            clearTimeout(timeout);
            timeout = setTimeout(searchWords, 300);
        });
    }
});