# 🎮 Unity ShadersMagic

In this project, I will present a variety of interesting mechanics implemented using shaders. It demonstrates how many complex and creative effects can be achieved with shaders. All implementations are developed in the Unity game engine.  

## 🔹 1 Animation Scroll
A simple shader is used to implement a scroll rolling and unrolling animation. A camera system is also added to highlight and focus on the scroll.

### 🎬 Demo

[![Watch the video](https://img.youtube.com/vi/9YdD3Ea4bso/0.jpg)](https://youtu.be/9YdD3Ea4bso)
---

## 🐦 2 GPU Flocking Simulation (Compute Shaders)

High-performance implementation of flocking (boids) behavior fully executed on the GPU using compute shaders.  
The system supports real-time simulation of thousands of agents with advanced behaviors such as target tracking and obstacle avoidance.

---
### ✨ Features

- 🚀 GPU-based simulation (Compute Shaders)
- 🧠 Classic flocking behaviors:
  - Alignment
  - Cohesion
  - Separation
- 🎯 Target following
- 🚧 Obstacle avoidance (collision prediction)
- ⚡ Scales to thousands of agents
- 🎮 Real-time parameter tuning

---
### 🛠 Technical Details

- Engine: **Unity**
- Language: **HLSL (Compute Shaders), C#**
- Data storage: **StructuredBuffers**
- Parallel execution: **1 thread per agent**
- Neighbor detection based on radius queries
- Optimized for GPU memory access and performance

---
### 📈 Performance

- Handles **5,000+ agents in real time**
- Minimal CPU usage
- Stable frame time due to full GPU parallelization

---
### 🧩 Architecture Overview

- Each agent stores:
  - Position
  - Velocity
  - Flock data (alignment, cohesion, separation)
- Simulation pipeline:
  1. Read neighbor data
  2. Compute flock forces
  3. Apply obstacle avoidance
  4. Update velocity and position

## 📜 License
MIT License.
