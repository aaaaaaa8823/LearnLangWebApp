let allLesson = [];

async function loadLessons() {
    const container = document.getElementById('lessonsContainer');
    if (!container) return;

    try {
        const token = localStorage.getItem('token');
        if (!token) {
            window.location.href = '/';
            return;
        }

        const response = await fetch('/api/Grammar/topics', {
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (!response.ok) throw new Error('Ошибка загрузки уроков');

        allLessons = await response.json();
        displayLessons(allLessons);

    } catch (error) {
        console.error('Ошибка:', error);
        container.innerHTML = '<div class="error">Ошибка загрузки уроков</div>';
    }
}

function displayLessons(lessons) {
    const container = document.getElementById('lessonContainer');

    if (!lessons || lessons.length === 0) {
        container.innerHTML = '<div class="empty">Уроки не найдены</div>';
        return;
    }

    container.innerHTML = lessons.map(lesson => `
        <div class="lesson-card" onclick="goToLesson(${lesson.id})">
            <div class="lesson-header">
                <span class="lesson-level">${escapeHtml(lesson.level)}</span>
            </div>
            <div class="lesson-body">
                <div class="lesson-title">${escapeHtml(lesson.title)}</div>
                <div class="lesson-description">${escapeHtml(lesson.description) || 'Описание отсутствует'}</div>
            </div>
            <div class="lesson-footer">
                <div class="tests-count">
                    ${lesson.tests && lesson.tests.length > 0 ? `📋 ${lesson.tests.length} тест(ов)` : '📖 Без тестов'}
                </div>
                <button class="btn-add" onclick="event.stopPropagation(); addToMyLessons(${lesson.id})">
                    + Добавить
                </button>
            </div>
        </div>
    `).join('');
}

function searchLessons() {
    const searchTerm = document.getElementById('lessonSearch').value.toLowerCase();

    if (!searchTerm) {
        displayLessons(allLessons);
        return;
    }

    const filtered = allLesson.filter(lesson => lesson.title.toLowerCase().includes(searchTerm)
    || (lesson.description && lesson.description.toLowerCase().includes(searchTerm))
    );

    displayLessons(filtered);
}

let searchTimeout;
function handleSearchInput() {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(searchLessons, 300);
}

function goToLesson(lessonId) {
    window.location.href = `/Home/LessonDetail?id=${lessonId}`;
}

document.addEventListener('DOMContentLoaded', () => {
    loadLessons();

    const searchInput = document.getElementById('lessonSearch');
    if (searchInput) {
        searchInput.addEventListener('input', handleSearchInput);
    }
});

//todo
async function addToMyLessons(lessonId) {
    try {
        const token = localStorage.getItem('token');

        const response = await fetch(`/api/Grammar/topic/${lessonId}/start`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Ошибка добавления урока');
        }

        alert('Урок добавлен в вашу коллекцию!');

        // Обновляем список уроков (чтобы обновить статус кнопки)
        await loadLessons();

    } catch (error) {
        console.error('Ошибка:', error);
        alert(error.message);
    }
}