import { mapComponent } from "./map.js";

await mapComponent();

setInterval(async () => await mapComponent(), 500)