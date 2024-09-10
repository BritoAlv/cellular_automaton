const armiesInput = document.querySelector('#armies');
const nextButton = document.querySelector('.next-button');

nextButton.addEventListener('click', () => {
    const armies = armiesInput.value;

    sessionStorage.setItem('armies', armies)
});