const obstacleInput = document.querySelector('#obstacle');
const nextButton = document.querySelector('.next-button');

nextButton.addEventListener('click', () => {
    const obstacleRate = obstacleInput.value;

    sessionStorage.setItem('obstacleRate', obstacleRate)
});
