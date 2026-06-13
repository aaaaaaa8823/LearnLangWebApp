let currentLesson = null;
let lessonType = null;
let currentSectionIndex = 0;
let sections = [];

const sectionConfig = [
    { id: 'theory', title: "Теория" },
    { id: 'examples', title: "Примеры" },
    { id: 'tests', title: "Тесты" },
    { id: 'complete', title: "Завершение" }
];

async function loadLesson() {
    const container = document.getElementById('lessonContainer');

    const urlParams = new URLSearchParams(window.location.search);
    const lessonId = urlParams.get('id');
    lessonType = urlParams.get('type');

    if (!lessonId) {
        container.innerHTML = '<div class="error">ID урока не указан</div>';
        console.log("ID урока не указан");
        return;
    }

    if (!lessonType) {
        lessonType = 'grammar';
    }

    try {
        const apiUrl = lessonType === 'grammar'
            ? `/api/Grammar/topic/${lessonId}`
            : `/api/Vocab/topic/${lessonId}`;

        const response = await fetch(apiUrl, {
            credentials: 'include'
        });

        if (response.status === 401) {
            console.log('Not authorized');
            container.innerHTML = '<div class="error">Необходимо войти в систему</div>';
            return;
        }

        if (!response.ok) {
            if (response.status === 404) {
                throw new Error('Урок не найден');
            }
            throw new Error('Ошибка загрузки урока');
        }

        currentLesson = await response.json();
        console.log('Урок загружен:', currentLesson);

        prepareSections();
        displayLesson();

    } catch (error) {
        console.error('Ошибка:', error);
        container.innerHTML = `<div class="error">${error.message}</div>`;
    }
}

function formatTheoryContent(content) {
    if (!content) return '<p>Нет содержания</p>';

    let html = content;

    html = html.replace(/^(.+?)\n={3,}$/gm, '<h1>$1</h1>');
    html = html.replace(/^(#{1,3})\s+(.+)$/gm, (match, hashes, text) => {
        const level = hashes.length;
        return `<h${level}>${text}</h${level}>`;
    });

    html = html.replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>');

 
    html = html.replace(/\*(.*?)\*/g, '<em>$1</em>');

    html = html.replace(/^- (.*)$/gm, '<li>$1</li>');
    html = html.replace(/(<li>.*<\/li>)/s, '<ul>$1</ul>');

    const paragraphs = html.split('\n\n');
    html = paragraphs.map(p => {
        if (p.startsWith('<h') || p.startsWith('<ul') || p.startsWith('<li')) {
            return p;
        }
        return `<p>${p.replace(/\n/g, '<br>')}</p>`;
    }).join('');

    return html;
}

function formatExamplesContent(content) {
    if (!content) return '<p>Нет примеров</p>';

    const lines = content.split('\n').filter(line => line.trim());
    if (lines.length === 0) return '<p>Нет примеров</p>';

    return `<ul class="examples-list">${lines.map(line => `<li>${escapeHtml(line)}</li>`).join('')}</ul>`;
}

function getDefaultTheoryContent(title) {
    return `# ${title}

Содержание урока будет добавлено позже.

Здесь будет представлена теория по данной теме с примерами и объяснениями.`;
}

function getDefaultExamplesContent(title) {
    return `Пример 1 с переводом
Пример 2 с переводом
Пример 3 с переводом`;
}

function prepareSections() {
    sections = [];

    const theoryContent = currentLesson.theoryContent || getDefaultTheoryContent(currentLesson.title);
    sections.push({
        id: 'theory',
        title: 'Теория',
        content: `<div class="theory-content">${formatTheoryContent(theoryContent)}</div>`
    });


    const examplesContent = currentLesson.examplesContent || getDefaultExamplesContent(currentLesson.title);
    sections.push({
        id: 'examples',
        title: 'Примеры',
        content: `<div class="examples-content">${formatExamplesContent(examplesContent)}</div>`
    });

    const testsHtml = currentLesson.tests && currentLesson.tests.length > 0
        ? `
            <div class="tests-list">
                ${currentLesson.tests.map(test => `
                    <div class="test-card">
                        <div class="test-info">
                            <span class="test-title">${escapeHtml(test.title)}</span>
                            <span class="test-details">
                                ${test.questionCount} вопросов | 
                                Проходной балл: ${test.passingScore}% | 
                                Время: ${test.timeLimitMinutes} мин
                            </span>
                        </div>
                        <button class="btn-test" onclick="startTest(${test.id})">
                            Пройти тест
                        </button>
                    </div>
                `).join('')}
            </div>
        `
        : '<p class="no-tests">К этому уроку пока нет тестов</p>';

    sections.push({
        id: 'tests',
        title: 'Тесты',
        content: testsHtml
    });

    const isCompleted = currentLesson.userProgress?.completed || false;
    const completedAt = currentLesson.userProgress?.completedAt;
    const completedDate = completedAt ? new Date(completedAt).toLocaleDateString('ru-RU') : '';

    const completeHtml = !isCompleted
        ? `
            <div class="complete-section">
                <p>Вы изучили материал урока? Отметьте его как пройденный!</p>
                <button class="btn-complete" onclick="markLessonComplete()">
                    Отметить как пройденный
                </button>
            </div>
        `
        : `
            <div class="completed-section">
                <div class="completed-message">
                    Урок пройден ${completedDate}!
                </div>
                <p>Вы можете повторить материал или перейти к следующему уроку.</p>
            </div>
        `;

    sections.push({
        id: 'complete',
        title: 'Завершение',
        content: completeHtml
    });
}

function displayLesson() {
    const container = document.getElementById('lessonContainer');

    if (!sections.length) {
        container.innerHTML = '<div class="error">Ошибка загрузки секций урока</div>';
        return;
    }

    const currentSection = sections[currentSectionIndex];
    const totalSections = sections.length;

    container.innerHTML = `
        <div class="carousel-container">
            <div class="carousel-header">
                <div class="progress-indicator">
                    ${sections.map((section, idx) => `
                        <div class="progress-dot ${idx === currentSectionIndex ? 'active' : ''} ${idx < currentSectionIndex ? 'completed' : ''}"
                             onclick="goToSection(${idx})">
                        </div>
                    `).join('')}
                </div>
                <div class="section-title">
                    <h2>${sectionConfig[currentSectionIndex]?.title || currentSection.title}</h2>
                </div>
            </div>
            
            <div class="carousel-content">
                <button class="carousel-nav prev" onclick="prevSection()" ${currentSectionIndex === 0 ? 'disabled' : ''}>
                     <i class="fas fa-chevron-left"></i> 
                </button>
                
                <div class="section-card">
                    <div class="section-content">
                        ${currentSection.content}
                    </div>
                </div>
                
                <button class="carousel-nav next" onclick="nextSection()" ${currentSectionIndex === totalSections - 1 ? 'disabled' : ''}>
                    <i class="fas fa-chevron-right"></i>
                </button>
            </div>
            
            <div class="carousel-footer">
                <div class="lesson-info">
                    <span class="lesson-level">Уровень: ${escapeHtml(currentLesson.level)}</span>
                    <span class="lesson-type">Тип: ${lessonType === 'vocab' ? 'Вокабуляр' : 'Грамматика'}</span>
                </div>
                <div class="carousel-pagination">
                    ${currentSectionIndex + 1} / ${totalSections}
                </div>
            </div>
        </div>
    `;
}

function nextSection() {
    if (currentSectionIndex < sections.length - 1) {
        currentSectionIndex++;
        displayLesson();
    }
}

function prevSection() {
    if (currentSectionIndex > 0) {
        currentSectionIndex--;
        displayLesson();
    }
}

function goToSection(index) {
    if (index >= 0 && index < sections.length) {
        currentSectionIndex = index;
        displayLesson();
    }
}

async function markLessonComplete() {
    if (!currentLesson) return;

    try {
        const apiUrl = lessonType === 'grammar'
            ? `/api/Grammar/topic/${currentLesson.id}/complete`
            : `/api/Vocab/topic/${currentLesson.id}/complete`;

        const response = await fetch(apiUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            credentials: 'include'
        });

        if (response.status === 401) {
            alert('Необходимо войти в систему');
            return;
        }

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Ошибка');
        }

        alert('Урок отмечен как пройденный!');
        window.location.reload();

    } catch (error) {
        console.error('Ошибка:', error);
        alert(error.message);
    }
}

function startTest(testId) {
    window.location.href = `/Home/TestDetail?id=${testId}&type=${lessonType}`;
}

function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

document.addEventListener('DOMContentLoaded', loadLesson);