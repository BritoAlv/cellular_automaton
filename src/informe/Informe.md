# Autómatas celulares para la modelación de batallas.

### Integrantes:

- Alvaro Luis González Brito
- Javier Lima García

### Autómata celular:

Un autómata celular está compuesto por células ( casillas, elementos ), en cada iteración o momento estas células poseen un estado. Para avanzar de iteración (momento) es actualizado el estado de cada célula ( casilla ), para determinar el próximo estado solamente es usada información de su estado actual y de el de sus vecinos en el momento. Es necesario tener definido entonces:

- Un conjunto de estados en los que puede encontrarse una casilla.
- Una función que dada una casilla devuelva las casillas que son vecinas a ella.
- Para cada casilla una función que dado su estado y el de sus casillas vecinas determine su próximo estado.
- Un estado inicial para cada casilla.
	
Teniendo definido lo anterior es posible realizar una simulación y observar la evolución de esta a lo largo del tiempo, simplemente empezando cada casilla en su estado inicial y en cada iteración actualizar los estados de las casillas usando la función de transición de cada casilla.

Lo impresionante de los autómatas celulares es como con simples funciones de transición se pueden obtener a lo largo de las iteraciones resultados no evidentes, hasta es capaz de realizar cómputo.

### Previos Usos e Historia
Los automatas celulares son un tablero de células que pueden estar en estados, siendo la cantidad de estados, discreta y finita, y cada célula usa las mismas reglas para, sincronizadamente, evolucionar, solamente interactuando con las casillas vecinas. Lo que diferencia uno de otro es el tipo de reglas usadas en la evolución. Pueden ser reglas lineales, no lineales, lógica difusa, o reglas genéticas. 

Una forma de definir estas reglas es a través de ecuaciones diferenciales, por ejemplo las leyes de Manchester, estas describen la dependencia con respecto al tiempo de las fuerzas de dos ejércitos $A,B$. Sería una forma de interpretar un modelo de presa $vs$ depredador. 

Este modelo simula una confrontación entre dos equipos que no tiene en cuenta la habilidad de cada equipo de obtener y usar la información sobre lo que sucede en el ambiente. Por ejemplo un equipo puede detectar que en una zona hay más tropas de su enemigo y decidir no dirigirse allí, o un equipo puede tener tropas que traicionan o desertan. O sea este tipo de información no está representada en las ecuaciones diferenciales.

Para resolver la situación anterior una posible idea es emplear un sistema experto para definir las reglas, o sea, cada casilla cuenta con reglas que usara para determinar su próximo estado, pero estas reglas incluyen razonamiento que pueda traer consigo, el uso de la información, por ejemplo si todos los vecinos de una casilla son del enemigo, esta casilla decide desertar o rendirse. Mientras que no se viole la condición de que la regla solamente use información de los vecinos de la casilla en cuestión. Otro aspecto a tener en cuenta de estas reglas es que deben ser capaces de mejorar o empeorar la situación a un equipo. O sea, debe haber tanto reglas que favorezcan a un equipo como reglas que lo perjudiquen.

Otra opción seria incorporar este tipo de heurística a las ecuaciones diferenciales, una forma de simular este tipo de comportamiento es a través de *Particle Swarm Optimization*, puesto que en este modelo cada casilla posee tres factores: reactividad, sociabilidad, y proactividad. En conjunto determinan su comportamiento. Este tipo de modelo es descrito en (2). Este consiste en una forma de modelar el comportamiento de los agentes a través de ecuaciones diferenciales.

Dado que existen varios métodos para modelar la simulación de combates usando automatas celulares, observamos que el objetivo en común es incorporar, en el modelo usado las *características* de un combate, tanto en las ecuaciones diferenciales como en el modelo usando el sistema experto, su eficacia estará determinada porque tan bien incluyen el comportamiento de una batalla en sus reglas. 

A continuación describimos una forma de implementar algunas reglas que incluyen comportamiento de lo sucedido en las batallas.

### Adaptado a la simulación de Batallas:

Usamos un tablero de $m*n$ en el que cada casilla es una célula de nuestro autómata, los estados de las casillas consisten en:

- un número indicando el equipo al que pertenece la casilla.
- un número *float* entre $0, 1$ que representa la fuerza de esa casilla.
- un número $1, 0$ que representa si la casilla está viva o no.

Un estado es una tripla con estos tres números. Teniendo esta definición de estados, lo próximo sería definir una función de transición que implemente heuristicas relacionada a lo que pasaría en una batalla.

Primero para representar los obstáculos usamos el número $-1$, como equipo, estos se mantienen como obstáculo durante toda la simulación.

Si los vecinos de una casilla son obstáculos o están muertos, esta mantiene su estado, se quisiera que sus casillas vecinas también escojan el color de este equipo, pero eso no es permitido en las reglas de los autómatas.

Para cada equipo sumamos las fuerzas de todas las casillas vecinas incluyéndolo a él que están vivas. Y así determinamos el equipo más fuerte en la vecindad. Dado que puede haber empate, escogemos uno de estos al azar.

Sobre el equipo más fuerte consideramos que son iguales los que están a una vecindad de 0.2 con respecto a el de mayor suma, porque dos equipos con fortaleza 5.3 o 5.33 deben ser iguales.

Tenemos dos casos la casilla en cuestión está viva o muerta:

#### Dead Cell

Si está muerta, habría una disputa entre sus vecinos para conquistarla, tiene sentido asignarle el equipo más fuerte entre sus vecinos.

Si hay un solo equipo entre los más fuertes, le asignamos este equipo y un promedio de esta fuerza ( era una suma de varios equipos que excede posiblemente a 1, el promedio garantiza que sea un número entre $0, 1$). Además le damos un pequeño aumento determinado al azar a su fuerza. Utilizando una ecuación del tipo $f' = f + (1-f) / (rnd() \mod 10 + 1)$, donde $f'$ es la nueva fuerza, $f$ es el promedio de la fuerza de el equipo seleccionado.

Si hay varios equipos escogemos uno al azar, pero disminuimos su fuerza ya que se supone que hubo disputa para conquistar la casilla, sería $f' = f / (rnd() \mod 10)$.

#### Alive Cell

Si entre los equipos más fuertes no se encuentra el de esta casilla, entonces la damos por muerta con fuerza $0$, en particular en la próxima iteración tomará valor de acuerdo a las reglas anteriores.

Si hay varios equipos fuertes, incluyendo a el de la casilla, mantenemos su equipo, pero disminuimos su fuerza en un por ciento aleatorio, o sea, $f' = f * rnd()$.

Ahora si el único equipo fuerte que hay es precisamente el de la casilla entonces tenemos dos casos:
	- entre los vecinos hay solamente casillas de este equipo, en este caso, aumentamos su fuerza : $f' = f + (1-f) * rnd()$. Esto simula que la casilla se fortalece.
	- en otro caso disminuimos su fuerza hallando la fuerza del equipo más fuerte, que este, y restándolas. Esto simula que hubo una batalla y la casilla resultó afectada aunque no muerta.   

### Notas

Lo anterior son heuristicas que tratan de simular lo que pasaría en las batallas, durante el proceso de una batalla, durante el proceso de determinar estas heuristicas; observamos lo importante que es aumentar y disminuir los parámetros para que haya aleatoriedad, y evitar que el proceso siempre converja a un resultado específico y parezca determinista, contrario a la idea de los autómatas celulares.

### Referencias
- Cellular Automata and Their Applications in Combat Modeling & Simulation, Deng Fang, Chen Jie, Chen Wenjie, Zhu Lin
- Partial differential equations versus cellular automata for modelling combat, Therese Keane
