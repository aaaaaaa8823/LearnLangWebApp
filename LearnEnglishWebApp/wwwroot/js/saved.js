let savedLessons = [];

async function loadSavedLessons() {
    const container = document.getElementById('savedContainer');
    if (!container) return;

    try {
        const token = localStorage.getItem('token');
        if (!token) {
            window.location.href = '/';
            return;
        }

        const response = await fetch('/api/Grammar/saved', {
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (!response.ok) {
            throw new Error('Ошибка загрузки сохраненных уроков');
        }

        savedLessons = await response.json();
        displaySavedLessons(savedLessons);

    } catch (error) {
        console.error('Ошибка:', error);
        container.innerHTML = '<div class="error">Ошибка загрузки сохраненных уроков</div>';
    }
}

function displaySavedLessons(lessons) {
    const container = document.getElementById('savedContainer');

    if (!lessons || lessons.length === 0) {
        container.innerHTML = '<div class="empty">У вас пока нет сохраненных уроков</div>';
        return;
    }

    container.innerHTML = lessons.map(lesson => `
        <div class="saved-card" data-lesson-id="${lesson.lessonId}">
            <div class="saved-header">
                <span class="saved-level">${escapeHtml(lesson.level)}</span>
                <span class="saved-date">Сохранено: ${new Date(lesson.savedAt).toLocaleDateString()}</span>
            </div>
            <div class="saved-title">${escapeHtml(lesson.title)}</div>
            <div class="saved-description">${escapeHtml(lesson.description) || 'Нет описания'}</div>
            <div class="saved-footer">
                <button class="btn-study" onclick="goToLesson(${lesson.lessonId})">
                    Изучать
                </button>
                <button class="btn-remove" onclick="removeSavedLesson(${lesson.lessonId})">
                    Удалить
                </button>
            </div>
        </div>
    `).join('');
}

function searchSavedLessons() {
    const searchInput = document.getElementById('savedSearch');
    if (!searchInput) return;

    const searchTerm = searchInput.value.toLowerCase();

    if (!searchTerm) {
        displaySavedLessons(savedLessons);
        return;
    }

    const filtered = savedLessons.filter(lesson =>
        lesson.title.toLowerCase().includes(searchTerm) ||
        (lesson.description && lesson.description.toLowerCase().includes(searchTerm))
    );
    displaySavedLessons(filtered);
}

function goToLesson(lessonId) {
    window.location.href = `/Home/LessonDetail?id=${lessonId}`;
}

async function removeSavedLesson(lessonId) {
    if (!confirm('Вы уверены, что хотите удалить этот урок из сохраненных?')) {
        return;
    }

    try {
        const token = localStorage.getItem('token');

        const response = await fetch(`/api/Grammar/topic/${lessonId}/unsave`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Ошибка удаления');
        }

        alert('Урок удален из сохраненных');

        await loadSavedLessons();

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
    searchTimeout = setTimeout(searchSavedLessons, 300);
}

document.addEventListener('DOMContentLoaded', () => {
    loadSavedLessons();

    const searchInput = document.getElementById('savedSearch');
    if (searchInput) {
        searchInput.addEventListener('input', handleSearchInput);
    }
});