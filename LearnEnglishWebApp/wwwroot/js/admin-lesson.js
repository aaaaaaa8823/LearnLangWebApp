let currentLessonType = 'grammar';

function getApiUrl() {
    const type = currentLessonType;
    return type === 'vocab' ? '/api/admin/vocab/lessons' : '/api/admin/grammar/topics';
}

async function loadLessons() {
    const container = document.getElementById('lessonsList');
    if (!container) return;

    try {
        const response = await fetch(getApiUrl(), { credentials: 'include' });
        if (!response.ok) throw new Error('Ошибка загрузки');

        const lessons = await response.json();
        displayLessons(lessons);

        const countEl = document.getElementById('lessonsCount');
        if (countEl) countEl.textContent = lessons.length;

    } catch (error) {
        console.error('Ошибка:', error);
        container.innerHTML = '<div class="empty">Ошибка загрузки уроков</div>';
    }
}

function parseMarkdown(text) {
    if (!text) return '';

    let html = text;

    html = html.replace(/^### (.*$)/gm, '<h3>$1</h3>');
    html = html.replace(/^## (.*$)/gm, '<h2>$1</h2>');
    html = html.replace(/^# (.*$)/gm, '<h1>$1</h1>');

    html = html.replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>');

    html = html.replace(/\*(.*?)\*/g, '<em>$1</em>');

    html = html.replace(/^- (.*$)/gm, '<li>$1</li>');
    html = html.replace(/(<li>.*<\/li>)/s, '<ul>$1</ul>');

    html = html.replace(/\n/g, '<br>');

    return html;
}

function togglePreview() {
    const preview = document.getElementById('theoryPreview');
    const textarea = document.getElementById('theoryContent');

    if (preview.style.display === 'none') {
        preview.innerHTML = parseMarkdown(textarea.value);
        preview.style.display = 'block';
    } else {
        preview.style.display = 'none';
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
                        <td>
                            <strong>${escapeHtml(lesson.title)}</strong>
                            ${lesson.description ? `<br><small>${escapeHtml(lesson.description.substring(0, 50))}${lesson.description.length > 50 ? '...' : ''}</small>` : ''}
                        </td>
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

async function editLesson(id) {
    try {
        const response = await fetch(`${getApiUrl()}/${id}`, { credentials: 'include' });
        if (!response.ok) throw new Error('Ошибка загрузки');

        const lesson = await response.json();

        document.getElementById('lessonId').value = lesson.id;
        document.getElementById('level').value = lesson.level;
        document.getElementById('title').value = lesson.title;
        document.getElementById('description').value = lesson.description || '';
        document.getElementById('orderIndex').value = lesson.orderIndex || 0;
        document.getElementById('theoryContent').value = lesson.theoryContent || '';
        document.getElementById('examplesContent').value = lesson.examplesContent || '';

        document.getElementById('formTitle').textContent = 'Редактировать урок';

 
        document.querySelector('.form-section').scrollIntoView({ behavior: 'smooth' });

    } catch (error) {
        showMessage(error.message, 'error');
    }
}

function formatText(type) {
    const textarea = document.getElementById('theoryContent');
    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const selectedText = textarea.value.substring(start, end);
    let wrappedText = '';

    switch (type) {
        case 'bold':
            wrappedText = `<strong>${selectedText}</strong>`;
            break;
        case 'italic':
            wrappedText = `<em>${selectedText}</em>`;
            break;
        case 'underline':
            wrappedText = `<u>${selectedText}</u>`;
            break;
        case 'h3':
            wrappedText = `<h3>${selectedText}</h3>`;
            break;
        case 'ul':
            const items = selectedText.split('\n').filter(line => line.trim());
            wrappedText = `<ul>\n${items.map(item => `  <li>${item}</li>`).join('\n')}\n</ul>`;
            break;
        case 'ol':
            const orderedItems = selectedText.split('\n').filter(line => line.trim());
            wrappedText = `<ol>\n${orderedItems.map(item => `  <li>${item}</li>`).join('\n')}\n</ol>`;
            break;
        default:
            wrappedText = selectedText;
    }

    textarea.value = textarea.value.substring(0, start) + wrappedText + textarea.value.substring(end);
}

const lessonForm = document.getElementById('lessonForm');
if (lessonForm) {
    lessonForm.addEventListener('submit', async (e) => {
        e.preventDefault();

        const id = document.getElementById('lessonId')?.value || '0';
        const level = document.getElementById('level')?.value;
        const title = document.getElementById('title')?.value.trim();
        const description = document.getElementById('description')?.value.trim() || '';
        const orderIndex = parseInt(document.getElementById('orderIndex')?.value) || 0;
        const theoryContent = document.getElementById('theoryContent')?.value || '';
        const examplesContent = document.getElementById('examplesContent')?.value || '';

        if (!title) {
            showMessage('Введите название урока', 'error');
            return;
        }

        const data = {
            level,
            title,
            description,
            orderIndex,
            theoryContent: theoryContent || null,
            examplesContent: examplesContent || null
        };

        const apiUrl = getApiUrl();

        try {
            const url = id !== '0' ? `${apiUrl}/${id}` : apiUrl;
            const method = id !== '0' ? 'PUT' : 'POST';

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
    document.getElementById('theoryContent').value = '';
    document.getElementById('examplesContent').value = '';
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

document.querySelectorAll('.type-btn').forEach(btn => {
    btn.addEventListener('click', (e) => {
        e.preventDefault();  

        const url = new URL(btn.href);
        const type = url.searchParams.get('type');

        if (type && type !== currentLessonType) {
            currentLessonType = type;

            const lessonTypeInput = document.getElementById('lessonType');
            if (lessonTypeInput) lessonTypeInput.value = type;

            document.querySelectorAll('.type-btn').forEach(b => {
                const btnType = new URL(b.href).searchParams.get('type');
                if (btnType === type) {
                    b.classList.add('active');
                } else {
                    b.classList.remove('active');
                }
            });

            resetForm();
            loadLessons();
        }
    });
});

document.addEventListener('DOMContentLoaded', () => {
    currentLessonType = document.getElementById('lessonType')?.value || 'grammar';
    loadLessons();
});