import { BattleFieldDisplayer } from "./battlefieldDisplayer.js";
import { URL } from "./globals.js";
import { Requester } from "./requester.js";

const width = sessionStorage.getItem('width');
const height = sessionStorage.getItem('height');
const armies = sessionStorage.getItem('armies');
const obstacleRate = sessionStorage.getItem('obstacleRate')

if (width == null || height == null || armies == null || obstacleRate == null)
    throw new Error("Invalid data from SessionStorage");

const battlefieldDisplayer = new BattleFieldDisplayer();
const requester = new Requester();

let response, matrix, status;
async function start() {
    response = await requester.post(
        `${URL}Start`,
        {
            "h": height,
            "w": width,
            "numberTeams": armies,
            "p": obstacleRate
        }
    );
    matrix = response.cells;
    status = response.percents;
};

start();

setInterval(async () => {
    battlefieldDisplayer.display(matrix, status);
    response = await requester.get(`${URL}UpdateCurrentState`);
    matrix = response.cells;
    status = response.percents;
}, 1000);

const restartButton = document.querySelector("#next-button");

restartButton.addEventListener("click", start);