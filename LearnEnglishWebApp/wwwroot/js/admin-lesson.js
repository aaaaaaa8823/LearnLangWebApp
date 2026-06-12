let currentLessonType = 'grammar';

function getApiUrl() {
    const type = document.getElementById('lessonType')?.value || currentLessonType;
    return type === 'vocab' ? '/api/admin/vocab/lessons' : '/api/admin/grammar/topics';
}

async function loadLessons() {
    const container = document.getElementById('lessonsList');
    currentLessonType = document.getElementById('lessonType')?.value || 'grammar';

    try {
        const response = await fetch(getApiUrl(), {
            credentials: 'include'
        });

        if (!response.ok) throw new Error('Ошибка загрузки');

        const lessons = await response.json();
        displayLessons(lessons);

        document.getElementById('lessonsCount').textContent = lessons.length;

    } catch (error) {
        console.error('Ошибка:', error);
        container.innerHTML = '<div class="empty">Ошибка загрузки уроков</div>';
    }
}

function displayLessons(lessons) {
    const container = document.getElementById('lessonsList');

    if (!lessons || lessons.length === 0) {
        container.innerHTML = '<div class="empty">Уроки не найдены</div>';
        return;
    }

    container.innerHTML = `
        <table class="data-table">
            <thead>
                <tr><th>Название</th><th>Уровень</th><th>Порядок</th><th>Тестов</th><th>Действия</th></tr>
            </thead>
            <tbody>
                ${lessons.map(lesson => `
                    <tr>
                        <td><strong>${escapeHtml(lesson.title)}</strong><br><small>${escapeHtml(lesson.description?.substring(0, 50) || '')}${lesson.description?.length > 50 ? '...' : ''}</small></td>
                        <td>${escapeHtml(lesson.level)}</td>
                        <td>${lesson.orderIndex || 0}</td>
                        <td>${lesson.tests?.length || 0}</td>
                        <td>
                            <button class="btn-edit" onclick="editLesson(${lesson.id})">Редактировать</button>
                            <button class="btn-delete" onclick="deleteLesson(${lesson.id})">Удалить</button>
                        </td>
                    </tr>
                `).join('')}
            </tbody>
        </table>
    `;
}

document.getElementById('lessonForm').addEventListener('submit', async (e) => {
    e.preventDefault();

    const id = document.getElementById('lessonId').value;
    const level = document.getElementById('level').value;
    const title = document.getElementById('title').value.trim();
    const description = document.getElementById('description').value.trim();
    const orderIndex = parseInt(document.getElementById('orderIndex').value) || 0;
    const lessonType = document.getElementById('lessonType').value;

    if (!title) {
        showMessage('Введите название урока', 'error');
        return;
    }

    const data = { level, title, description, orderIndex };
    const apiUrl = getApiUrl();

    try {
        const url = id && id !== '0'
            ? `${apiUrl}/${id}`
            : apiUrl;

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

        showMessage('Урок успешно сохранён!', 'success');
        resetForm();
        loadLessons();

    } catch (error) {
        showMessage(error.message, 'error');
    }
});

async function editLesson(id) {
    try {
        const response = await fetch(`${getApiUrl()}/${id}`, {
            credentials: 'include'
        });

        if (!response.ok) throw new Error('Ошибка загрузки');

        const lesson = await response.json();

        document.getElementById('lessonId').value = lesson.id;
        document.getElementById('level').value = lesson.level;
        document.getElementById('title').value = lesson.title;
        document.getElementById('description').value = lesson.description || '';
        document.getElementById('orderIndex').value = lesson.orderIndex || 0;

        document.getElementById('formTitle').textContent = '✏️ Редактировать урок';

        document.querySelector('.form-section').scrollIntoView({ behavior: 'smooth' });

    } catch (error) {
        showMessage(error.message, 'error');
    }
}

async function deleteLesson(id) {
    if (!confirm('Вы уверены, что хотите удалить этот урок? Все связанные тесты также будут удалены.')) return;

    try {
        const response = await fetch(`${getApiUrl()}/${id}`, {
            method: 'DELETE',
            credentials: 'include'
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Ошибка удаления');
        }

        showMessage('Урок удалён', 'success');
        loadLessons();

    } catch (error) {
        showMessage(error.message, 'error');
    }
}

function resetForm() {
    document.getElementById('lessonId').value = '0';
    document.getElementById('level').value = 'A1';
    document.getElementById('title').value = '';
    document.getElementById('description').value = '';
    document.getElementById('orderIndex').value = '0';
    document.getElementById('formTitle').textContent = '➕ Добавить новый урок';
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

function observeLessonTypeChange() {
    const typeInput = document.getElementById('lessonType');
    if (typeInput) {
        const observer = new MutationObserver(() => {
            loadLessons();
            resetForm();
        });
        observer.observe(typeInput, { attributes: true, attributeFilter: ['value'] });
    }
}

document.addEventListener('DOMContentLoaded', () => {
    loadLessons();
    observeLessonTypeChange();
});

