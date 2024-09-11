import { COLORS } from "./globals.js";

export class BattleFieldDisplayer {
    #cellSize;
    #canvas;
    constructor(width, height) {
        this.#cellSize = 30;
        this.#canvas = document.querySelector('canvas');
        this.#canvas.width = width * this.#cellSize;
        this.#canvas.height = height * this.#cellSize;
    }

    display(matrix, status) {
        this.#displayMatrix(matrix);
        this.#displayStatus(status);
    }

    #displayMatrix(matrix) {
        if (matrix.length <= 0 || matrix[0].length <= 0)
            throw new Error("Height and width must be greater than zero");
        
        const cellSize = 30;
        
        // Width should be matrix[0].length and height should be matrix.length
        const width = matrix.length;
        const height = matrix[0].length;

        const canvas = document.querySelector('canvas');
        canvas.width = width * cellSize;
        canvas.height = height * cellSize;
        const ctx = canvas.getContext('2d');
        
        const textColor = 'black'; 
        const fontSize = 13;
        ctx.font = `${fontSize}px Arial`;
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';

        for (let i = 0; i < width; i++) {
            for (let j = 0; j < height; j++) {
                const army = matrix[i][j].id;
                const strength = `${matrix[i][j].value}`.slice(0, 4);

                const x = i * cellSize;
                const y = j * cellSize;
                const color = this.#getColor(army);
                ctx.fillStyle = color;
                ctx.fillRect(x, y, cellSize, cellSize);

                // Set text properties
                ctx.fillStyle = textColor;
                const text = `${strength}`;
                ctx.fillText(text, x + cellSize / 2, y + cellSize / 2); // Draw text in the center of the cell
            }
        }
    }

    #displayStatus(status) {
        const players = document.querySelector('#players');
        
        players.innerHTML = "";
        for (let i = 0; i < status.length; i++) {
            const current = `${status[i]}`.slice(0, 4);
            players.innerHTML += `<li>Army ${i}: ${current}%</li>`
        }
    }

    #getColor(armyId) {
        return armyId == -1 ? 'grey' : COLORS[armyId];
    }
}