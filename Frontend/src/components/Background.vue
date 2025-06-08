<script setup lang="ts">
import { ref, onMounted } from 'vue';

interface Bubble { id:number; style:string }
const palette = ['#A7BED3', '#C6E2E9'];  // бажані кольори
const N = 10;
const bubbles = ref<Bubble[]>([]);

onMounted(() => {
  const arr:Bubble[] = [];

  for (let i=0; i<N; i++){
    // розмір (60-400 px) та “яскравість”
    const size = 60 + Math.random()*340;
    const opacity = 0.3 + (size/400)*0.5;

    // стартова позиція всередині екрана
    const startX = Math.random()*100;   // vw
    const startY = Math.random()*100;   // vh

    // випадковий вектор руху (±20 vw, ±20 vh)
    const dx = -20 + Math.random()*40;  // vw
    const dy = -20 + Math.random()*40;  // vh

    // тривалість (15–35 с)
    const dur = 15 + Math.random()*20;

    // колір → r,g,b
    const hex = palette[Math.floor(Math.random()*palette.length)];
    const rgb = parseInt(hex.slice(1),16);
    const r = (rgb>>16)&255, g=(rgb>>8)&255, b=rgb&255;

    arr.push({
      id:i,
      style:`
        --size:${size}px;
        --o:${opacity};
        --c:${r},${g},${b};
        --x:${startX}vw;
        --y:${startY}vh;
        --dx:${dx}vw;
        --dy:${dy}vh;
        --t:${dur}s;
      `
    });
  }
  bubbles.value = arr;
});
</script>

<template>
  <div class="bubbles">
    <div v-for="b in bubbles" :key="b.id" class="bubble" :style="b.style"></div>
  </div>
</template>
