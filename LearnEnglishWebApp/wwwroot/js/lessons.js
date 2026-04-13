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

        const response = await fetch('/api/Grammar/topics', {
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Ошибка загрузки уроков');
        }

        allLessons = await response.json();
        console.log('Загружено уроков:', allLessons.length);
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

    if (!container) {
        console.error('Контейнер lessonsContainer не найден');
        return;
    }

    if (!lessons || lessons.length === 0) {
        container.innerHTML = '<div class="empty">Уроки не найдены</div>';
        return;
    }

    container.innerHTML = lessons.map(lesson => `
        <div class="lesson-card" onclick="goToLesson(${lesson.id})">
            <div class="lesson-header">
                <span class="lesson-level">${escapeHtml(lesson.level)}</span>
                ${lesson.isCompleted ? '<span class="completed-badge">✓ Пройдено</span>' : ''}
            </div>
            <div class="lesson-body">
                <div class="lesson-title">${escapeHtml(lesson.title)}</div>
                <div class="lesson-description">${escapeHtml(lesson.description) || 'Описание отсутствует'}</div>
            </div>
            <div class="lesson-footer">
                <div class="tests-count">
                    ${lesson.tests && lesson.tests.length > 0 ? `${lesson.tests.length} тест(ов)` : 'Без тестов'}
                </div>
                <button class="btn-add" onclick="event.stopPropagation(); addToMyLessons(${lesson.id})">
                    ${lesson.isCompleted ? 'Пройдено' : '+ Добавить'}
                </button>
            </div>
        </div>
    `).join('');
}

function searchLessons() {
    const searchInput = document.getElementById('lessonSearch');
    if (!searchInput) return;

    const searchTerm = searchInput.value.toLowerCase();

    if (!searchTerm) {
        displayLessons(allLessons);
        return;
    }

    const filtered = allLessons.filter(lesson =>
        lesson.title.toLowerCase().includes(searchTerm) ||
        (lesson.description && lesson.description.toLowerCase().includes(searchTerm))
    );
    displayLessons(filtered);
}

function goToLesson(lessonId) {
    window.location.href = `/Home/LessonDetail?id=${lessonId}`;
}

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

        // Перезагружаем список уроков
        await loadLessons();

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

let searchTimeout;
function handleSearchInput() {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(searchLessons, 300);
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        loadLessons();

        const searchInput = document.getElementById('lessonSearch');
        if (searchInput) {
            searchInput.addEventListener('input', handleSearchInput);
        }
    });
} else {
    loadLessons();

    const searchInput = document.getElementById('lessonSearch');
    if (searchInput) {
        searchInput.addEventListener('input', handleSearchInput);
    }
}