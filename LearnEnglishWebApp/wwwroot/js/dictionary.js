let allWords = [];
let learningWords = 0;
let learnedWords = 0;
let currentModalStatus = null

async function loadUserWordStats() {
    try {
        const response = await fetch('/api/Profile/stats', {
            credentials: 'include'
        });

        if (response.ok) {
            const stats = await response.json();
            learningWords = stats.learningWords || 0;
            learnedWords = stats.learnedWords || 0;

            const learningElement = document.getElementById('learningWords');
            const learnedElement = document.getElementById('learnedWords');

            if (learningElement) learningElement.textContent = learningWords;
            if (learnedElement) learnedElement.textContent = learnedWords;
        } else if (response.status === 401) {
            console.log('Not authorized');
        }
    } catch (error) {
        console.error('Ошибка загрузки статистики слов:', error);
    }
}

async function showWordsModal(status) {
    currentModalStatus = status;
    const modal = document.getElementById('wordsModal');
    const modalTitle = document.getElementById('modalTitle');
    const modalBody = document.getElementById('modalBody');

    if (status === 'learning') {
        modalTitle.textContent = 'Слова в процессе изучения';
    } else {
        modalTitle.textContent = 'Выученные слова';
    }

    modal.style.display = 'block';
    modalBody.innerHTML = '<div class="loading-modal">Загрузка...</div>';

    await loadUserWordsByStatus(status);
}

function closeModal() {
    const modal = document.getElementById('wordsModal');
    modal.style.display = 'none';
    currentModalStatus = null;
}

window.onclick = function (event) {
    const modal = document.getElementById('wordsModal');
    if (event.target === modal) {
        closeModal();
    }
}

async function loadUserWordsByStatus(status) {
    try {
        const response = await fetch(`/api/UserWords/my-words?status=${status}`, {
            credentials: 'include'
        });

        if (response.status === 401) {
            console.log('Not authorized');
            return;
        }

        if (!response.ok) throw new Error('Ошибка загрузки слов');

        const words = await response.json();
        displayUserWords(words, status);

    } catch (error) {
        console.error('Ошибка:', error);
        const modalBody = document.getElementById('modalBody');
        modalBody.innerHTML = '<div class="error">Ошибка загрузки слов</div>';
    }
}

async function loadDictionaryWords() {
    const container = document.getElementById('wordsList');
    if (!container) return;

    try {
        const response = await fetch('/api/Dictionary/all', {
            credentials: 'include'
        });

        if (response.status === 401) {
            console.log('Not authorized');
            return;
        }

        if (!response.ok) throw new Error('Ошибка загрузки слов');

        allWords = await response.json();
        displayWords(allWords);

    } catch (error) {
        console.error('Ошибка:', error);
        container.innerHTML = '<div class="error">Ошибка загрузки слов</div>';
    }
}

function displayUserWords(words, status) {
    const modalBody = document.getElementById('modalBody');

    if (!words || words.length === 0) {
        modalBody.innerHTML = '<div class="empty-modal">Нет слов</div>';
        return;
    }

    modalBody.innerHTML = `
        <div class="modal-word-list">
            ${words.map(word => `
                <div class="modal-word-item" data-word-id="${word.wordId}">
                    <div class="modal-word-info">
                        <span class="modal-word-text">${escapeHtml(word.word)}</span>
                        <span class="modal-word-translation">${escapeHtml(word.translations || word.translation || '')}</span>
                        <span class="modal-word-status ${status === 'learning' ? 'status-learning-badge' : 'status-learned-badge'}">
                            ${status === 'learning' ? 'В процессе' : 'Выучено'}
                        </span>
                    </div>
                    <div class="modal-word-actions">
                        ${status === 'learning' ? `
                            <button class="btn-modal btn-learned" onclick="markAsLearned(${word.wordId}, '${escapeHtml(word.word)}')">
                                Выучено
                            </button>
                        ` : `
                            <button class="btn-modal btn-learning" onclick="markAsLearning(${word.wordId}, '${escapeHtml(word.word)}')">
                                Повторить
                            </button>
                        `}
                        <button class="btn-modal btn-delete" onclick="removeWord(${word.wordId}, '${escapeHtml(word.word)}')">
                            Удалить
                        </button>
                    </div>
                </div>
            `).join('')}
        </div>
    `;
}

async function markAsLearned(wordId, wordText) {
    try {
        const user = JSON.parse(localStorage.getItem('user'));

        const response = await fetch(`/api/UserWords/update-status`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                userId: user.id,
                wordId: wordId,
                status: 'learned'
            }),
            credentials: 'include'
        });

        if (!response.ok) throw new Error('Ошибка обновления статуса');

        learningWords--;
        learnedWords++;

        const learningElement = document.getElementById('learningWords');
        const learnedElement = document.getElementById('learnedWords');

        if (learningElement) learningElement.textContent = learningWords;
        if (learnedElement) learnedElement.textContent = learnedWords;

        alert(`Слово "${wordText}" отмечено как выученное!`);

        if (currentModalStatus === 'learning') {
            await loadUserWordsByStatus('learning');
        }

    } catch (error) {
        console.error('Ошибка:', error);
        alert(error.message);
    }
}

async function markAsLearning(wordId, wordText) {
    try {
        const user = JSON.parse(localStorage.getItem('user'));

        const response = await fetch(`/api/UserWords/update-status`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                userId: user.id,
                wordId: wordId,
                status: 'learning'
            }),
            credentials: 'include'
        });

        if (!response.ok) throw new Error('Ошибка обновления статуса');

        learningWords++;
        learnedWords--;

        const learningElement = document.getElementById('learningWords');
        const learnedElement = document.getElementById('learnedWords');

        if (learningElement) learningElement.textContent = learningWords;
        if (learnedElement) learnedElement.textContent = learnedWords;

        alert(`Слово "${wordText}" возвращено в процесс изучения!`);

        if (currentModalStatus === 'learned') {
            await loadUserWordsByStatus('learned');
        }

    } catch (error) {
        console.error('Ошибка:', error);
        alert(error.message);
    }
}

async function removeWord(wordId, wordText) {
    if (!confirm(`Вы уверены, что хотите удалить слово "${wordText}" из вашего словаря?`)) {
        return;
    }

    try {
        const user = JSON.parse(localStorage.getItem('user'));

        const response = await fetch(`/api/UserWords/remove`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                userId: user.id,
                wordId: wordId
            }),
            credentials: 'include'
        });

        if (!response.ok) throw new Error('Ошибка удаления слова');

        if (currentModalStatus === 'learning') {
            learningWords--;
        } else if (currentModalStatus === 'learned') {
            learnedWords--;
        }

        const learningElement = document.getElementById('learningWords');
        const learnedElement = document.getElementById('learnedWords');

        if (learningElement) learningElement.textContent = learningWords;
        if (learnedElement) learnedElement.textContent = learnedWords;

        alert(`Слово "${wordText}" удалено из вашего словаря!`);

        await loadUserWordsByStatus(currentModalStatus);

    } catch (error) {
        console.error('Ошибка:', error);
        alert(error.message);
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
                    Добавить
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
        const response = await fetch(`/api/Dictionary/search?q=${encodeURIComponent(searchTerm)}`, {
            credentials: 'include'
        });

        if (response.status === 401) {
            console.log('Not authorized');
            return;
        }

        if (!response.ok) throw new Error('Ошибка поиска');

        const filteredWords = await response.json();
        displayWords(filteredWords);

    } catch (error) {
        console.error('Ошибка поиска:', error);
    }
}

async function addWordToLearning(wordId, wordText) {
    try {
        const userStr = localStorage.getItem('user');
        if (!userStr) {
            alert('Данные пользователя не найдены');
            return;
        }

        const user = JSON.parse(userStr);

        const response = await fetch('/api/UserWords/add', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                userId: user.id,
                wordId: wordId,
                status: 'learning'
            }),
            credentials: 'include'
        });

        if (response.status === 401) {
            alert('Необходимо войти в систему');
            return;
        }

        if (!response.ok) {
            let errorMessage = 'Ошибка добавления слова';
            try {
                const error = await response.json();
                errorMessage = error.message || errorMessage;
            } catch (e) {
                const text = await response.text();
                if (text) errorMessage = text;
            }
            throw new Error(errorMessage);
        }

        learningWords++;
        const learningElement = document.getElementById('learningWords');
        if (learningElement) learningElement.textContent = learningWords;

        alert(`Слово "${wordText}" добавлено в список "В процессе"`);

        const wordItem = document.querySelector(`.word-item[data-word-id="${wordId}"]`);
        if (wordItem) {
            const button = wordItem.querySelector('.btn-add');
            if (button) {
                button.textContent = 'Добавлено';
                button.disabled = true;
                button.style.background = '#4caf50';
            }
        }

    } catch (error) {
        console.error('Ошибка:', error);
        alert(error.message);
    }
}

function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
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