let currentTest = null;
let currentQuestionIndex = 0;
let userAnswer = [];
let startTime = null;
let timeInterval = null;
let timeSpent = 0;

async function loadTest() {
    const container = document.getElementById('testContainer');

    const urlParams = new URLSearchParams(window.location.search);
    const testId = urlParams.get('id');
    const testType = urlParams.get('type') || 'grammar';

    if (!testId) {
        container.innerHTML = '<div class="error">ID теста не указан</div>';
        return;
    }

    try {
        const token = localStorage.getItem('token');
        if (!token) {
            window.location.href = '/';
            return;
        }

        const apiUrl = testType === 'grammar'
            ? `/api/Grammar/test/${testId}`
            : `/api/Vocab/test/${testId}`;

        const response = await fetch(apiUrl, {
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (!response.ok) {
            throw new Error('Ошибка загрузки теста');
        }

        currentTest = await response.json();
        startTime = Date.now();
        startTimer();
        displayQuestion();

    } catch (error) {
        console.error('Ошибка:', error);
        container.innerHTML = `<div class="error">${error.message}</div>`;
    }
}

function startTimer() {
    if(timeInterval) clearInterval(timeInterval);

    timerInterval = setInterval(() => {
        const elapsed = Math.floor((Date.now() - startTime) / 1000);
        const remaining = Math.max(0, (currentTest.timeLimitMinutes * 60) - elapsed);
        updateTimerDisplay(remaining);

        if (remaining <= 0) {
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
                    <div class="timer-value" id="timerDisplay">${formatTime(currentTest.timeLimitMinutes * 60)}</div>
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
                                ${userAnswer[currentQuestionIndex]?.selectedOption === idx
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
    if (!userAnswer[currentQuestionIndex] || userAnswer[currentQuestionIndex].selectedOption === indefined) {
        alert('Пожалуйста, выберите ответ');
        return;
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

async function submitTest(){
    clearInterval(timerInterval);

    const timeSpentSeconds = Math.floor((Date.now() - startTime) / 1000);
    let correctCount = 0;
    let totalScore = 0;
    let maxScore = 0;

    const answer = currentTest.question.map((question, idx => {
        const userAnswer = userAnswers[idx];
        const isCorrect = userAnswer && userAnswer.selectedOption === question.correctOption;
        const pointsEarned = isCorrect ? question.points : 0;

        if (isCorrect) correctCount++;
        totalScore += pointsEarned;
        maxScore += question.points;

        return {
            questionId: question.id,
            userAnswer: userAnswer ? question.options[userAnswer.selectedOption] : '',
            correctAnswer: question.options[question.correctOption],
            isCorrect: isCorrect,
            pointsEarned: pointsEarned,
            maxPoints: question.points
        };
    }))

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
        const token = localStorage.getItem('token');
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
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(resultData)
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
                                <span class="answer-status">${answer.isCorrect ? '✓ Правильно' : '✗ Неправильно'}</span>
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

function getMockQuestions() {
    return [
        {
            id: 1,
            text: "I ___ to school every day.",
            options: ["go", "goes", "going", "went"],
            correctOption: 0,
            points: 2
        },
        {
            id: 2,
            text: "She ___ English very well.",
            options: ["speak", "speaks", "speaking", "is speak"],
            correctOption: 1,
            points: 2
        },
        {
            id: 3,
            text: "They ___ playing football now.",
            options: ["is", "am", "are", "be"],
            correctOption: 2,
            points: 2
        },
        {
            id: 4,
            text: "He ___ to the cinema yesterday.",
            options: ["go", "goes", "went", "gone"],
            correctOption: 2,
            points: 2
        },
        {
            id: 5,
            text: "We ___ a new car next week.",
            options: ["buy", "buys", "will buy", "bought"],
            correctOption: 2,
            points: 2
        }
    ];
}

// TODO: Временно мок-вопросы, пока не сделаю админскую часть
document.addEventListener('DOMContentLoaded', async () => {
    await loadTest();
    if (currentTest && !currentTest.questions) {
        currentTest.questions = getMockQuestions();
        currentTest.questionCount = currentTest.questions.length;
        currentTest.passingScore = 60;
        currentTest.timeLimitMinutes = 10;
        displayQuestion();
    }
});