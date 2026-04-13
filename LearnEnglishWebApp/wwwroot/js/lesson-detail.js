let currentLesson = null;

async function loadLesson() {
    const container = document.getElementById('lessonContainer');

    const urlParams = new URLSearchParams(window.location.search);
    const lessonId = urlParams.get('id');

    if (!lessonId) {
        container.innerHTML = '<div class="error">ID урока не указан</div>';
        console.log("ID урока не указан");
        return;
    }

    try {
        const token = localStorage.getItem('token');
        if (!token) {
            window.location.href = '/';
            return;
        }

        const response = await fetch(`/api/Grammar/topic/${lessonId}`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (!response.ok) {
            if (response.status === 404) {
                throw new Error('Урок не найден');
            }
            throw new Error('Ошибка загрузки урока');
        }

        currentLesson = await response.json();
        console.log('Урок загружен:', currentLesson);

        displayLesson();

    } catch (error) {
        console.error('Ошибка:', error);
        container.innerHTML = `<div class="error">${error.message}</div>`;
    }
}

function displayLesson() {
    const container = document.getElementById('lessonContainer');

    if (!currentLesson) {
        container.innerHTML = '<div class="error">Данные урока не загружены</div>';
        return;
    }

    const isCompleted = currentLesson.userProgress?.completed || false;
    const completedAt = currentLesson.userProgress?.completedAt;

    const completedDate = completedAt ? new Date(completedAt).toLocaleDateString('ru-RU') : '';

    // Временный контент (в будущем можно загружать из отдельного JSON файла)
    const theoryContent = getTheoryContent(currentLesson.title);
    const examplesContent = getExamplesContent(currentLesson.title);

    container.innerHTML = `
        <div class="lesson-container">
            <div class="lesson-header">
                <h1>${escapeHtml(currentLesson.title)}</h1>
                <div class="lesson-level">Уровень: ${escapeHtml(currentLesson.level)}</div>
            </div>
            
            <div class="lesson-body">
                <div class="theory-section">
                    <h2>📖 Теория</h2>
                    <div class="theory-content">
                        ${theoryContent}
                    </div>
                </div>
                
                <div class="examples-section">
                    <h2>📝 Примеры</h2>
                    <ul class="examples-list">
                        ${examplesContent.map(ex => `<li>${escapeHtml(ex)}</li>`).join('')}
                    </ul>
                </div>
            </div>
            
            <div class="lesson-footer">
                ${!isCompleted ? `
                    <button class="btn-complete" onclick="markLessonComplete()">
                        Отметить как пройденный
                    </button>
                ` : `
                    <div class="completed-message">
                        ✓ Урок пройден ${completedDate}
                    </div>
                `}
                
                ${currentLesson.tests && currentLesson.tests.length > 0 ? `
                    <div class="available-tests">
                        <h3>📋 Доступные тесты:</h3>
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
                    </div>
                ` : `
                    <div class="available-tests">
                        <p style="color: #6b7a8f; text-align: center;">К этому уроку пока нет тестов</p>
                    </div>
                `}
            </div>
        </div>
    `;
}

async function markLessonComplete() {
    if (!currentLesson) return;

    try {
        const token = localStorage.getItem('token');

        const response = await fetch(`/api/Grammar/topic/${currentLesson.id}/complete`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

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
    window.location.href = `/Home/TestDetail?id=${testId}`;
}

function getExamplesContent(title) {
    const examplesMap = {
        'Present Simple': [
            'I work every day. (Я работаю каждый день)',
            'She works in an office. (Она работает в офисе)',
            'Water boils at 100 degrees. (Вода кипит при 100 градусах)',
            'The train leaves at 6 PM. (Поезд отправляется в 6 вечера)'
        ],
        'Past Simple': [
            'I worked yesterday. (Я работал вчера)',
            'She went to London last year. (Она ездила в Лондон в прошлом году)',
            'They watched a movie yesterday. (Они смотрели фильм вчера)'
        ],
        'Future Simple': [
            'I will call you tomorrow. (Я позвоню тебе завтра)',
            'She will be here soon. (Она скоро будет здесь)',
            'It will rain later. (Позже пойдет дождь)'
        ]
    };

    return examplesMap[title] || [
        'Пример 1 с переводом',
        'Пример 2 с переводом',
        'Пример 3 с переводом'
    ];
}

function getTheoryContent(title) {
    const contentMap = {
        'Present Simple': `
            <p><strong>Present Simple (настоящее простое время)</strong> используется для обозначения:</p>
            <ul>
                <li>Обычных, повторяющихся действий</li>
                <li>Фактов и общих истин</li>
                <li>Расписаний и графиков</li>
            </ul>
            <h3>Образование утвердительных предложений:</h3>
            <p>I/You/We/They + глагол (без окончания)<br>
            He/She/It + глагол + s/es</p>
            <h3>Образование отрицательных предложений:</h3>
            <p>I/You/We/They + do not (don't) + глагол<br>
            He/She/It + does not (doesn't) + глагол</p>
            <h3>Образование вопросительных предложений:</h3>
            <p>Do/Does + подлежащее + глагол?</p>
        `,
        'Past Simple': `
            <p><strong>Past Simple (прошедшее простое время)</strong> используется для обозначения:</p>
            <ul>
                <li>Действий, которые произошли в прошлом</li>
                <li>Последовательных действий в прошлом</li>
            </ul>
            <h3>Правильные глаголы:</h3>
            <p>глагол + ed (work → worked)</p>
            <h3>Неправильные глаголы:</h3>
            <p>используется 2-я форма глагола (go → went)</p>
        `,
        'Future Simple': `
            <p><strong>Future Simple (будущее простое время)</strong> используется для обозначения:</p>
            <ul>
                <li>Действий, которые произойдут в будущем</li>
                <li>Спонтанных решений</li>
                <li>Предсказаний</li>
            </ul>
            <h3>Образование:</h3>
            <p>will + глагол (без частицы to)</p>
        `
    };

    return contentMap[title] || `
        <p>Содержание урока "${title}" будет добавлено позже.</p>
        <p>Здесь будет представлена теория по данной теме с примерами и объяснениями.</p>
    `;
}


document.addEventListener('DOMContentLoaded', loadLesson);