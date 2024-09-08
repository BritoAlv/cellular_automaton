import { URL } from "./globals.js";
import { Requester } from "./requester.js";

export async function mapComponent() {
    const requester = new Requester()
    const map = await requester.get(URL)
    const rows = map.length
    const columns = map[0].length 

    const body = document.querySelector("body");

    body.innerHTML = `<div class="grid-container"></div>`;

    const gridContainer = document.querySelector('.grid-container');

    gridContainer.style.gridTemplateColumns = `repeat(${columns}, 1fr)`;

    for (let i = 0; i < rows; i++) {
        for (let j = 0; j < columns; j++)
        {
            gridContainer.innerHTML += `<div class="grid-item" style="background-color: ${map[i][j]}"></div>`;
        }
    }
}