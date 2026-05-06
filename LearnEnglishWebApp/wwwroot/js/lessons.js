console.log('lessons.js loaded');

let allLessons = [];

async function loadLessons() {
    console.log('loadLessons started');

    const container = document.getElementById('lessonsContainer');
    if (!container) {
        console.error('Контейнер lessonsContainer не найден на странице');
        return;
    }

    try {
        const grammarResponse = await fetch('/api/Grammar/topics', {
            credentials: 'include'
        });

        if (grammarResponse.status === 401) {
            console.log('Not authorized');
            return;
        }

        if (!grammarResponse.ok) {
            throw new Error('Ошибка загрузки грамматических уроков');
        }

        let grammarLessons = await grammarResponse.json();

        const vocabResponse = await fetch('/api/Vocab/topics', {
            credentials: 'include'
        });

        if (vocabResponse.status === 401) {
            console.log('Not authorized');
            return;
        }

        if (!vocabResponse.ok) {
            throw new Error('Ошибка загрузки вокабулярных уроков');
        }

        let vocabLessons = await vocabResponse.json();

        const savedGrammarResponse = await fetch('/api/Grammar/saved', {
            credentials: 'include'
        });

        if (savedGrammarResponse.ok) {
            const savedLessons = await savedGrammarResponse.json();
            const savedIds = new Set(savedLessons.map(l => l.lessonId));
            grammarLessons = grammarLessons.map(lesson => ({ ...lesson, type: 'grammar', isSaved: savedIds.has(lesson.id) }));
        } else {
            grammarLessons = grammarLessons.map(lesson => ({ ...lesson, type: 'grammar', isSaved: false }));
        }

        const savedVocabResponse = await fetch('/api/Vocab/saved', {
            credentials: 'include'
        });

        if (savedVocabResponse.ok) {
            const savedVocabLessons = await savedVocabResponse.json();
            const savedVocabIds = new Set(savedVocabLessons.map(l => l.lessonId));
            vocabLessons = vocabLessons.map(lesson => ({ ...lesson, type: 'vocab', isSaved: savedVocabIds.has(lesson.id) }));
        } else {
            vocabLessons = vocabLessons.map(lesson => ({ ...lesson, type: 'vocab', isSaved: false }));
        }

        allLessons = [...grammarLessons, ...vocabLessons];

        const levelOrder = { 'A1': 1, 'A2': 2, 'B1': 3, 'B2': 4, 'C1': 5, 'C2': 6 };
        allLessons.sort((a, b) => {
            const levelCompare = (levelOrder[a.level] || 0) - (levelOrder[b.level] || 0);
            if (levelCompare !== 0) return levelCompare;
            return (a.orderIndex || 0) - (b.orderIndex || 0);
        });

        console.log('Загружено уроков:', allLessons.length);
        displayLessons(allLessons);

    } catch (error) {
        console.error('Ошибка:', error);
        container.innerHTML = '<div class="error">Ошибка загрузки уроков: ' + error.message + '</div>';
    }
}

function displayLessons(lessons) {
    const container = document.getElementById('lessonsContainer');

    if (!lessons || lessons.length === 0) {
        container.innerHTML = '<div class="empty">Уроки не найдены</div>';
        return;
    }

    container.innerHTML = lessons.map(lesson => {
        const buttonHtml = lesson.isSaved
            ? `<button class="btn-add saved" style="background:#4caf50;" disabled>Сохранено</button>`
            : `<button class="btn-add" onclick="event.stopPropagation(); saveLesson(${lesson.id}, '${lesson.type}')">Сохранить</button>`;

        return `
            <div class="lesson-card" onclick="goToLesson(${lesson.id}, '${lesson.type}')">
                <div class="lesson-header">
                    <span class="lesson-level">${escapeHtml(lesson.level)}</span>
                    ${lesson.isCompleted ? '<span class="completed-badge">Пройдено</span>' : ''}
                </div>
                <div class="lesson-body">
                    <div class="lesson-title">${escapeHtml(lesson.title)}</div>
                    <div class="lesson-description">${escapeHtml(lesson.description) || 'Описание отсутствует'}</div>
                </div>
                <div class="lesson-footer">
                    <div class="tests-count">${lesson.tests && lesson.tests.length > 0 ? `${lesson.tests.length} тест(ов)` : 'Без тестов'}</div>
                    ${buttonHtml}
                </div>
            </div>
        `;
    }).join('');
}

async function saveLesson(lessonId, lessonType) {
    try {
        const apiUrl = lessonType === 'grammar' ? `/api/Grammar/topic/${lessonId}/save` : `/api/Vocab/topic/${lessonId}/save`;
        const response = await fetch(apiUrl, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({}), credentials: 'include' });

        if (!response.ok) throw new Error('Ошибка сохранения урока');
        alert('Урок сохранен!');
        await loadLessons();
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
    const filtered = allLessons.filter(lesson => lesson.title.toLowerCase().includes(searchTerm) || (lesson.description && lesson.description.toLowerCase().includes(searchTerm)));
    displayLessons(filtered);
}

function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

document.addEventListener('DOMContentLoaded', async () => {
    console.log('DOM loaded, loading lessons...');
    await loadLessons();

    const searchInput = document.getElementById('lessonSearch');
    if (searchInput) {
        let timeout;
        searchInput.addEventListener('input', () => {
            clearTimeout(timeout);
            timeout = setTimeout(searchLessons, 300);
        });
    }
});