# Autómatas celulares para la modelación de batallas.

### Integrantes:

- Alvaro Luis González Brito
- Javier Lima García

### Autómata celular:

Un autómata celular está compuesto por células ( casillas, elementos ), en cada iteración o momento estas células poseen un estado. Para avanzar de iteración (momento) es actualizado el estado de cada célula ( casilla ), para determinar el próximo estado solamente es usada información de su estado actual y de el de sus vecinos en el momento. Es necesario tener definido entonces:

	- un conjunto de estados en los que puede encontrarse una casilla.
	- una función que dada una casilla devuelva las casillas que son vecinas a ella.
	- para cada casilla una función que dado su estado y el de sus casillas vecinas determine su próximo estado.
	- un estado inicial para cada casilla.
	
Teniendo definido lo anterior es posible realizar una simulación y observar la evolución de esta a lo largo del tiempo, simplemente empezando cada casilla en su estado inicial y en cada iteración actualizar los estados de las casillas usando la función de transición de cada casilla.

Lo impresionante de los autómatas celulares es como con simples funciones de transición se pueden obtener a lo largo de las iteraciones resultados no evidentes, hasta es capaz de realizar computo.

### Adaptado a la simulación de Batallas:

Usamos un tablero de $m*n$ en el que cada casilla es una célula de nuestro autómata, los estados de las casillas consisten en:
	- un número indicando el equipo al que pertenece la casilla.
	- un número *float* entre $0, 1$ que representa la fuerza de esa casilla.
	- un número $1, 0$ que representa si la casilla está viva o no.

Un estado es una tripla con estos tres números. Teniendo esta definición de estados, lo próximo sería definir una función de transición que implemente heurísticas relacionada a lo que pasaría en una batalla.

Primero para representar los obstáculos usamos el número $-1$, como equipo, estos se mantienen como obstáculo durante toda la simulación.

Si los vecinos de una casilla son obstáculos o están muertos, esta mantiene su estado, se quisiera que sus casillas vecinas también escogan el color de este equipo, pero eso no es permitido en las reglas de los autómatas.

Para cada equipo sumamos las fuerzas de todas las casillas vecinas incluyéndolo a él que están vivas. Y así determinamos el equipo más fuerte en la vecindad. Dado que puede haber empate, escogemos uno de estos al azar.

Sobre el equipo más fuerte consideramos que son iguales los que están a una vecindad de 0.2 con respecto a el de mayor suma, porque dos equipos con fortaleza 5.3 o 5.33 deben ser iguales.

Tenemos dos casos la casilla en cuestión está viva o muerta:

#### Dead Cell

Si está muerta, habría una disputa entre sus vecinos para conquistarla, tiene sentido asignarle el equipo de el equipo más fuerte entre sus vecinos.

Si hay un solo equipo entre los más fuertes, le asignamos este equipo y un promedio de esta fuerza ( era una suma de varios equipos que excede posiblemente a 1, el promedio garantiza que sea un número entre $0, 1$). Además le damos un pequeño aumento determinado al azar a su fuerza. Utilizando una ecuación del tipo $f' = f + (1-f) / (rnd() \mod 10 + 1)$, donde $f'$ es la nueva fuerza, $f$ es el promedio de la fuerza de el equipo seleccionado.

Si hay varios equipos escogemos uno al azar, pero disminuimos su fuerza ya que se supone que hubo disputa para conquistar la casilla, sería $f' = f / (rnd() \mod 10)$.

#### Alive Cell

Si entre los equipos más fuertes no se encuentra el de esta casilla, entonces la damos por muerta con fuerza $0$, en particular en la próxima iteración tomará valor de acuerdo a las reglas anteriores.

Si hay varios equipos fuertes, incluyendo a el de la casilla, mantenemos su equipo, pero disminuimos su fuerza en un porciento aleatorio, o sea, $f' = f * rnd()$.

Ahora si el único equipo fuerte que hay es precisamente el de la casilla entonces tenemos dos casos:
	- entre los vecinos hay solamente casillas de este equipo, en este caso, aumentamos su fuerza : $f' = f + (1-f) * rnd()$. Esto simula que la casilla se fortalece.
	- en otro caso disminuimos su fuerza hallando la fuerza del equipo más fuerte, que este, y restándolas. Esto simula que hubo una batalla y la casilla resultó afectada aunque no muerta.   

### Notas

Lo anterior son heurísticas que tratan de simular lo que pasaría en las batallas, durante el proceso de una batalla, durante el proceso de determinar estas observamos lo importante que es aumentar y disminuir los parámetros para que haya aleatoriedad, y evitar que el proceso siempre converga a un resultado específico y parezca determinista, contrario a la idea de los autómatas celulares.