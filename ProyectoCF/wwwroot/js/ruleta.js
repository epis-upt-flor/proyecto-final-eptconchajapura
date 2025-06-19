// wwwroot/js/ruleta.js
class Ruleta {
    constructor(canvasId) {
        this.canvas = document.getElementById(canvasId);
        this.ctx = this.canvas.getContext('2d');
        this.secciones = ['Fácil', 'Normal', 'Difícil'];
        this.colores = ['#4CAF50', '#2196F3', '#F44336'];
        this.angulo = 0;
        this.girando = false;

        this.dibujar();
    }

    dibujar() {
        const { ctx, canvas, secciones, colores } = this;
        const radio = canvas.width / 2;
        const anguloSeccion = (2 * Math.PI) / secciones.length;

        ctx.clearRect(0, 0, canvas.width, canvas.height);

        secciones.forEach((seccion, i) => {
            ctx.beginPath();
            ctx.fillStyle = colores[i];
            ctx.moveTo(radio, radio);
            ctx.arc(radio, radio, radio, i * anguloSeccion, (i + 1) * anguloSeccion);
            ctx.fill();

            ctx.save();
            ctx.translate(radio, radio);
            ctx.rotate(i * anguloSeccion + anguloSeccion / 2);
            ctx.textAlign = 'right';
            ctx.fillStyle = '#fff';
            ctx.font = '16px Arial';
            ctx.fillText(seccion, radio - 10, 5);
            ctx.restore();
        });

        ctx.beginPath();
        ctx.fillStyle = '#bb86fc';
        ctx.moveTo(radio * 2 - 10, radio - 5);
        ctx.lineTo(radio * 2 - 10, radio + 5);
        ctx.lineTo(radio * 2, radio);
        ctx.fill();
    }

    girar() {
        return new Promise((resolve) => {
            if (this.girando) return;

            this.girando = true;
            const duracion = 3000;
            const startTime = Date.now();
            const vueltas = 5 + Math.random() * 3;

            const animar = () => {
                const elapsed = Date.now() - startTime; // ← Corregido aquí
                const progress = Math.min(elapsed / duracion, 1);
                const easing = Math.sin(progress * Math.PI / 2);

                this.angulo = easing * vueltas * Math.PI * 2;
                this.dibujar();

                if (progress < 1) {
                    requestAnimationFrame(animar);
                } else {
                    this.girando = false;
                    const seccionIndex = Math.floor(
                        (this.angulo % (2 * Math.PI)) / ((2 * Math.PI) / this.secciones.length)
                    ); // ← Corregido aquí

                    resolve(this.secciones[this.secciones.length - 1 - seccionIndex]);
                }
            };

            requestAnimationFrame(animar);
        });
    }


    requestAnimationFrame(animar);
});
    }
}