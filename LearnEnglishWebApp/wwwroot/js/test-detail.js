let currentTest = null;
let currentQuestionIndex = 0;
let userAnswers = [];
let startTime = null;
let timerInterval = null;
let timeSpent = 0;
let currentRemainingSeconds = 0;

async function loadTest() {
    const container = document.getElementById('testContainer');

    const urlParams = new URLSearchParams(window.location.search);
    const testId = urlParams.get('id');
    const testType = urlParams.get('type') || 'grammar';

    if (!testId) {
        console.log('ID теста не указан');
        container.innerHTML = '<div class="error">ID теста не указан</div>';
        return;
    }

    try {
        const apiUrl = testType === 'grammar'
            ? `/api/Grammar/test/${testId}`
            : `/api/Vocab/test/${testId}`;

        const response = await fetch(apiUrl, {
            credentials: 'include'
        });

        if (response.status === 401) {
            console.log('Not authorized');
            container.innerHTML = '<div class="error">Необходимо войти в систему</div>';
            return;
        }

        if (!response.ok) {
            throw new Error('Ошибка загрузки теста');
        }

        currentTest = await response.json();

        // Проверяем, есть ли вопросы
        if (!currentTest.questions || currentTest.questions.length === 0) {
            console.log('В тесте нет вопросов');
            container.innerHTML = '<div class="error">В тесте нет вопросов. Обратитесь к администратору.</div>';
            return;
        }

        console.log('Загружен тест:', currentTest);
        console.log('Количество вопросов:', currentTest.questions.length);

        currentTest.questionCount = currentTest.questions.length;
        currentRemainingSeconds = currentTest.timeLimitMinutes * 60;
        startTime = Date.now();
        startTimer();

        displayQuestion();

    } catch (error) {
        console.error('Ошибка:', error);
        container.innerHTML = `<div class="error">${error.message}</div>`;
    }
}

function startTimer() {
    if (timerInterval) clearInterval(timerInterval);

    timerInterval = setInterval(() => {
        const elapsed = Math.floor((Date.now() - startTime) / 1000);
        currentRemainingSeconds = Math.max(0, (currentTest.timeLimitMinutes * 60) - elapsed);

        updateTimerDisplay(currentRemainingSeconds);

        if (currentRemainingSeconds <= 0) {
            clearInterval(timerInterval);
            submitTest();
        }
    }, 1000);
}

function updateTimerDisplay(seconds) {
    const timerElement = document.getElementById('timerDisplay');
    if (!timerElement) return;

    const minutes = Math.floor(seconds / 60);
    const secs = seconds % 60;
    timerElement.textContent = `${minutes.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;

    if (seconds <= 60) {
        timerElement.classList.add('warning');
    } else {
        timerElement.classList.remove('warning');
    }
}

function displayQuestion() {
    const container = document.getElementById('testContainer');

    if (!currentTest || !currentTest.questions || currentTest.questions.length === 0) {
        container.innerHTML = '<div class="error">Вопросы не найдены</div>';
        return;
    }

    const question = currentTest.questions[currentQuestionIndex];
    const totalQuestions = currentTest.questions.length;
    const progress = ((currentQuestionIndex + 1) / totalQuestions) * 100;

    const currentTimeDisplay = formatTime(currentRemainingSeconds);

    container.innerHTML = `
        <div class="test-container">
            <div class="test-header">
                <div class="test-title-info">
                    <h1>${escapeHtml(currentTest.title)}</h1>
                    <div class="test-meta">
                        <span class="test-level">Уровень: ${escapeHtml(currentTest.level)}</span>
                        <span class="test-passing">Проходной балл: ${currentTest.passingScore}%</span>
                    </div>
                </div>
                <div class="timer-section">
                    <div class="timer-label">Осталось времени:</div>
                    <div class="timer-value" id="timerDisplay">${currentTimeDisplay}</div>
                </div>
            </div>
            
            <div class="progress-bar">
                <div class="progress-fill" style="width: ${progress}%"></div>
            </div>
            <div class="progress-text">Вопрос ${currentQuestionIndex + 1} из ${totalQuestions}</div>
            
            <div class="question-container">
                <h3 class="question-text">${escapeHtml(question.text)}</h3>
                <div class="options-list">
                    ${question.options.map((option, idx) => `
                        <div class="option-item" onclick="selectOption(${idx})">
                            <div class="option-radio" id="option_${idx}">
                                ${userAnswers[currentQuestionIndex]?.selectedOption === idx
            ? '<i class="fas fa-dot-circle" style="color: #ff9800;"></i>'
            : '<i class="far fa-circle"></i>'}
                            </div>
                            <div class="option-text">${escapeHtml(option)}</div>
                        </div>
                    `).join('')}
                </div>
            </div>
            
            <div class="test-footer">
                <button class="nav-btn prev" onclick="prevQuestion()" ${currentQuestionIndex === 0 ? 'disabled' : ''}>
                    <i class="fas fa-chevron-left"></i> 
                </button>
                <button class="nav-btn next" onclick="nextQuestion()">
                    <i class="fas fa-chevron-right"></i>
                </button>
            </div>
        </div>
    `;
}

function selectOption(optionIndex) {
    if (!userAnswers[currentQuestionIndex]) {
        userAnswers[currentQuestionIndex] = {};
    }
    userAnswers[currentQuestionIndex].selectedOption = optionIndex;
    displayQuestion();
}

function prevQuestion() {
    if (currentQuestionIndex > 0) {
        currentQuestionIndex--;
        displayQuestion();
    }
}

function nextQuestion() {
    const hasAnswer = userAnswers[currentQuestionIndex] && userAnswers[currentQuestionIndex].selectedOption !== undefined;

    if (!hasAnswer) {
        if (!confirm('Вы не ответили на вопрос. Пропустить?')) {
            return;
        }
    }

    if (currentQuestionIndex < currentTest.questions.length - 1) {
        currentQuestionIndex++;
        displayQuestion();
    } else {
        if (confirm('Вы уверены, что хотите завершить тест?')) {
            submitTest();
        }
    }
}

async function submitTest() {
    clearInterval(timerInterval);

    const timeSpentSeconds = Math.floor((Date.now() - startTime) / 1000);
    let correctCount = 0;
    let totalScore = 0;
    let maxScore = 0;

    const answers = currentTest.questions.map((question, idx) => {
        const userAnswer = userAnswers[idx];
        const isCorrect = userAnswer && userAnswer.selectedOption !== undefined &&
            userAnswer.selectedOption === question.correctOption;
        const pointsEarned = isCorrect ? question.points : 0;

        if (isCorrect) correctCount++;
        totalScore += pointsEarned;
        maxScore += question.points;

        return {
            questionId: question.id,
            userAnswer: userAnswer && userAnswer.selectedOption !== undefined
                ? question.options[userAnswer.selectedOption]
                : 'Пропущен',
            correctAnswer: question.options[question.correctOption],
            isCorrect: isCorrect,
            pointsEarned: pointsEarned,
            maxPoints: question.points
        };
    });

    const percentage = (totalScore / maxScore) * 100;
    const isPassed = percentage >= currentTest.passingScore;

    showResult({
        totalScore: totalScore,
        maxScore: maxScore,
        percentage: percentage,
        isPassed: isPassed,
        correctCount: correctCount,
        totalQuestions: currentTest.questions.length,
        timeSpentSeconds: timeSpentSeconds,
        answers: answers
    });

    try {
        const testType = new URLSearchParams(window.location.search).get('type') || 'grammar';

        const resultData = {
            testId: currentTest.id,
            score: totalScore,
            maxScore: maxScore,
            percentage: percentage,
            timeSpentSeconds: timeSpentSeconds,
            correctAnswers: correctCount,
            wrongAnswers: currentTest.questions.length - correctCount,
            answers: answers
        };

        const apiUrl = testType === 'grammar'
            ? '/api/Grammar/submit'
            : '/api/Vocab/submit';

        await fetch(apiUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(resultData),
            credentials: 'include'
        });

    } catch (error) {
        console.error('Ошибка сохранения результата:', error);
    }
}

function showResult(result) {
    const container = document.getElementById('testContainer');
    const isPassed = result.isPassed;

    container.innerHTML = `
        <div class="result-container">
            <div class="result-header ${isPassed ? 'passed' : 'failed'}">
                <h1>${isPassed ? 'Поздравляем!' : 'Попробуйте еще раз'}</h1>
                <p>${isPassed ? 'Вы успешно прошли тест!' : 'К сожалению, вы не набрали проходной балл'}</p>
            </div>
            
            <div class="result-stats">
                <div class="stat-card">
                    <div class="stat-value">${result.correctCount} / ${result.totalQuestions}</div>
                    <div class="stat-label">Правильных ответов</div>
                </div>
                <div class="stat-card">
                    <div class="stat-value">${Math.round(result.percentage)}%</div>
                    <div class="stat-label">Результат</div>
                </div>
                <div class="stat-card">
                    <div class="stat-value">${formatTime(result.timeSpentSeconds)}</div>
                    <div class="stat-label">Затрачено времени</div>
                </div>
            </div>
            
            <div class="result-details">
                <h3>Детали ответов:</h3>
                <div class="answers-list">
                    ${result.answers.map((answer, idx) => `
                        <div class="answer-item ${answer.isCorrect ? 'correct' : 'incorrect'}">
                            <div class="answer-header">
                                <span class="question-num">Вопрос ${idx + 1}</span>
                                <span class="answer-status">${answer.isCorrect ? 'Правильно' : 'Неправильно'}</span>
                                <span class="answer-points">${answer.pointsEarned}/${answer.maxPoints} баллов</span>
                            </div>
                            <div class="answer-details">
                                <div class="user-answer">Ваш ответ: ${escapeHtml(answer.userAnswer || 'Не выбран')}</div>
                                <div class="correct-answer">Правильный ответ: ${escapeHtml(answer.correctAnswer)}</div>
                            </div>
                        </div>
                    `).join('')}
                </div>
            </div>
            
            <div class="result-actions">
                <button class="btn-retry" onclick="retryTest()">Пройти заново</button>
                <button class="btn-back" onclick="goBack()">Вернуться к тестам</button>
            </div>
        </div>
    `;
}

function retryTest() {
    currentQuestionIndex = 0;
    userAnswers = [];
    startTime = Date.now();
    startTimer();
    displayQuestion();
}

function goBack() {
    window.location.href = '/Home/Tests';
}

function formatTime(seconds) {
    const minutes = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${minutes.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
}

function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

document.addEventListener('DOMContentLoaded', async () => {
    await loadTest();
});