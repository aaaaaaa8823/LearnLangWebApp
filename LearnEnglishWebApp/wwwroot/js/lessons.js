let allLessons = [];

async function loadLessons() {
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', loadLessons);
        return;
    }

    const container = document.getElementById('lessonsContainer');

    if (!container) {
        console.error('Контейнер lessonsContainer не найден на странице');
        return;
    }

    try {
        const token = localStorage.getItem('token');
        if (!token) {
            window.location.href = '/';
            return;
        }

        const grammarResponse = await fetch('/api/Grammar/topics', {
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (!grammarResponse.ok) {
            throw new Error('Ошибка загрузки грамматических уроков');
        }

        let grammarLessons = await grammarResponse.json();

        const vocabResponse = await fetch('/api/Vocab/topics', {
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (!vocabResponse.ok) {
            throw new Error('Ошибка загрузки вокабулярных уроков');
        }

        let vocabLessons = await vocabResponse.json();

        const savedGrammarResponse = await fetch('/api/Grammar/saved', {
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (savedGrammarResponse.ok) {
            const savedLessons = await savedGrammarResponse.json();
            const savedIds = new Set(savedLessons.map(l => l.lessonId));

            grammarLessons = grammarLessons.map(lesson => ({
                ...lesson,
                type: 'grammar',
                isSaved: savedIds.has(lesson.id)
            }));
        } else {
            grammarLessons = grammarLessons.map(lesson => ({
                ...lesson,
                type: 'grammar',
                isSaved: false
            }));
        }

        const savedVocabResponse = await fetch('/api/Vocab/saved', {
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (savedVocabResponse.ok) {
            const savedVocabLessons = await savedVocabResponse.json();
            const savedVocabIds = new Set(savedVocabLessons.map(l => l.lessonId));

            vocabLessons = vocabLessons.map(lesson => ({
                ...lesson,
                type: 'vocab',
                isSaved: savedVocabIds.has(lesson.id)
            }));
        } else {
            vocabLessons = vocabLessons.map(lesson => ({
                ...lesson,
                type: 'vocab',
                isSaved: false
            }));
        }

        allLessons = [...grammarLessons, ...vocabLessons];

        const levelOrder = { 'A1': 1, 'A2': 2, 'B1': 3, 'B2': 4, 'C1': 5, 'C2': 6 };
        allLessons.sort((a, b) => {
            const levelCompare = (levelOrder[a.level] || 0) - (levelOrder[b.level] || 0);
            if (levelCompare !== 0) return levelCompare;
            return (a.orderIndex || 0) - (b.orderIndex || 0);
        });

        console.log('Загружено уроков:', allLessons.length);
        console.log('Грамматика:', grammarLessons.length);
        console.log('Вокабуляр:', vocabLessons.length);
        displayLessons(allLessons);

    } catch (error) {
        console.error('Ошибка:', error);
        if (container) {
            container.innerHTML = '<div class="error">Ошибка загрузки уроков: ' + error.message + '</div>';
        }
    }
}

function displayLessons(lessons) {
    const container = document.getElementById('lessonsContainer');

    if (!lessons || lessons.length === 0) {
        container.innerHTML = '<div class="empty">Уроки не найдены</div>';
        return;
    }

    container.innerHTML = lessons.map(lesson => {
        const typeBadge = lesson.type === 'vocab'
            ? '<span class="lesson-type vocab">Вокабуляр</span>'
            : '<span class="lesson-type grammar">Грамматика</span>';

        let buttonHtml = '';

        if (lesson.isSaved) {
            buttonHtml = `<button class="btn-add saved" onclick="event.stopPropagation(); unsaveLesson(${lesson.id}, '${lesson.type}')" style="background:#4caf50;">Сохранено</button>`;
        } else {
            buttonHtml = `<button class="btn-add" onclick="event.stopPropagation(); saveLesson(${lesson.id}, '${lesson.type}')">Сохранить</button>`;
        }

        return `
            <div class="lesson-card" onclick="goToLesson(${lesson.id}, '${lesson.type}')">
                <div class="lesson-header">
                    <span class="lesson-level">${escapeHtml(lesson.level)}</span>
                    ${typeBadge}
                    ${lesson.isCompleted ? '<span class="completed-badge">Пройдено</span>' : ''}
                </div>
                <div class="lesson-body">
                    <div class="lesson-title">${escapeHtml(lesson.title)}</div>
                    <div class="lesson-description">${escapeHtml(lesson.description) || 'Описание отсутствует'}</div>
                </div>
                <div class="lesson-footer">
                    <div class="tests-count">
                        ${lesson.tests && lesson.tests.length > 0 ? `${lesson.tests.length} тест(ов)` : 'Без тестов'}
                    </div>
                    ${buttonHtml}
                </div>
            </div>
        `;
    }).join('');
}

async function unsaveLesson(lessonId, lessonType) {
    if (!confirm('Удалить урок из сохраненных?')) {
        return;
    }

    try {
        const token = localStorage.getItem('token');
        if (!token) {
            alert('Необходимо войти в систему');
            window.location.href = '/';
            return;
        }

        const apiUrl = lessonType === 'grammar'
            ? `/api/Grammar/topic/${lessonId}/unsave`
            : `/api/Vocab/topic/${lessonId}/unsave`;

        const response = await fetch(apiUrl, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Ошибка удаления из сохраненных');
        }

        alert('Урок удален из сохраненных');

        const lessonIndex = allLessons.findIndex(l => l.id === lessonId && l.type === lessonType);
        if (lessonIndex !== -1) {
            allLessons[lessonIndex].isSaved = false;
        }

        displayLessons(allLessons);

    } catch (error) {
        console.error('Ошибка:', error);
        alert(error.message);
    }
}

async function saveLesson(lessonId, lessonType) {
    try {
        const token = localStorage.getItem('token');
        if (!token) {
            alert('Необходимо войти в систему');
            window.location.href = '/';
            return;
        }

        const apiUrl = lessonType === 'grammar'
            ? `/api/Grammar/topic/${lessonId}/save`
            : `/api/Vocab/topic/${lessonId}/save`;

        const response = await fetch(apiUrl, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Ошибка сохранения урока');
        }

        alert('Урок сохранен! Вы можете найти его в разделе "Сохраненные"');

        const lessonIndex = allLessons.findIndex(l => l.id === lessonId && l.type === lessonType);
        if (lessonIndex !== -1) {
            allLessons[lessonIndex].isSaved = true;
        }

        displayLessons(allLessons);

    } catch (error) {
        console.error('Ошибка:', error);
        alert(error.message);
    }
}

function goToLesson(lessonId, lessonType) {
    window.location.href = `/Home/LessonDetail?id=${lessonId}&type=${lessonType}`;
}

function searchLessons() {
    const searchTerm = document.getElementById('lessonSearch').value.toLowerCase();

    if (!searchTerm) {
        displayLessons(allLessons);
        return;
    }

    const filtered = allLessons.filter(lesson =>
        lesson.title.toLowerCase().includes(searchTerm) ||
        (lesson.description && lesson.description.toLowerCase().includes(searchTerm)) ||
        lesson.type.toLowerCase().includes(searchTerm)
    );
    displayLessons(filtered);
}

function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

let searchTimeout;
function handleSearchInput() {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(searchLessons, 300);
}

document.addEventListener('DOMContentLoaded', () => {
    loadLessons();

    const searchInput = document.getElementById('lessonSearch');
    if (searchInput) {
        searchInput.addEventListener('input', handleSearchInput);
    }
});