const widthInput = document.querySelector('#width');
const heightInput = document.querySelector('#height');
const nextButton = document.querySelector('.next-button');

nextButton.addEventListener('click', () => {
    const width = widthInput.value;
    const height = heightInput.value;

    sessionStorage.setItem('width', width)
    sessionStorage.setItem('height', height)
});
