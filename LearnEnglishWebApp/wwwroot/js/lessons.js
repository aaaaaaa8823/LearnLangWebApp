let allLessons = [];

// lessons.js - обновленная функция loadLessons

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

        const lessonsResponse = await fetch('/api/Grammar/topics', {
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (!lessonsResponse.ok) {
            const error = await lessonsResponse.json();
            throw new Error(error.message || 'Ошибка загрузки уроков');
        }

        let lessons = await lessonsResponse.json();

        const savedResponse = await fetch('/api/Grammar/saved', {
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (savedResponse.ok) {
            const savedLessons = await savedResponse.json();
            const savedIds = new Set(savedLessons.map(l => l.lessonId));

            lessons = lessons.map(lesson => ({
                ...lesson,
                isSaved: savedIds.has(lesson.id)
            }));
        }

        allLessons = lessons;
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

    container.innerHTML = lessons.map(lesson => {
        let buttonHtml = '';

        if (lesson.isSaved) {
            buttonHtml = `<button class="btn-add saved" onclick="event.stopPropagation(); unsaveLesson(${lesson.id})" style="background:#4caf50;">✓ Сохранено</button>`;
        } else {
            buttonHtml = `<button class="btn-add" onclick="event.stopPropagation(); saveLesson(${lesson.id})">+ Сохранить</button>`;
        }

        return `
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
                    ${buttonHtml}
                </div>
            </div>
        `;
    }).join('');
}

async function unsaveLesson(lessonId) {
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

        const response = await fetch(`/api/Grammar/topic/${lessonId}/unsave`, {
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

        const lessonIndex = allLessons.findIndex(l => l.id === lessonId);
        if (lessonIndex !== -1) {
            allLessons[lessonIndex].isSaved = false;
        }

        displayLessons(allLessons);

    } catch (error) {
        console.error('Ошибка:', error);
        alert(error.message);
    }
}

async function saveLesson(lessonId) {
    try {
        const token = localStorage.getItem('token');
        if (!token) {
            alert('Необходимо войти в систему');
            window.location.href = '/';
            return;
        }

        const response = await fetch(`/api/Grammar/topic/${lessonId}/save`, {
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

        // Обновляем статус урока
        const lessonIndex = allLessons.findIndex(l => l.id === lessonId);
        if (lessonIndex !== -1) {
            allLessons[lessonIndex].isSaved = true;
        }

        displayLessons(allLessons);

    } catch (error) {
        console.error('Ошибка:', error);
        alert(error.message);
    }
}

function goToLesson(lessonId) {
    window.location.href = `/Home/LessonDetail?id=${lessonId}`;
}


async function addToMyLessons(lessonId) {
    try {
        const token = localStorage.getItem('token');
        if (!token) {
            alert('Необходимо войти в систему');
            window.location.href = '/';
            return;
        }

        const response = await fetch(`/api/Grammar/topic/${lessonId}/save`, {
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

        const savedLesson = await response.json();
        console.log('Урок сохранен:', savedLesson);

        alert('Урок сохранен! Вы можете найти его в разделе "Сохраненные"');

        const button = event.target;
        button.textContent = '✓ Сохранено';
        button.classList.add('added');
        button.disabled = true;

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