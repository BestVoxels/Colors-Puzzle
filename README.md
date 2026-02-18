# 🧩 Color Puzzle — Algorithmic Logic Engine

A mobile cognitive puzzle game driven by **Recursive Spatial Subdivision**.
The system scales difficulty dynamically by mathematically refining the grid resolution (**O(N²) complexity**) while maintaining stable performance on legacy hardware.

---

## 🧠 Core Engineering Concepts

### **Recursive Spatial Subdivision**
The game does not simply spawn random squares. It uses a recursive algorithm that mathematically bisects the grid coordinates:
1.  **Level 1:** 3x3 Grid (Low Complexity)
2.  **Level 10:** 6x6 Grid (Medium Complexity)
3.  **Level 20:** 12x12 Grid (High Complexity)

### **Polymorphic Logic Engine**
The validation system switches runtime rules dynamically based on the current mode:
- **Frequency Analysis:** "Count X items" ($O(N)$)
- **Inverse Key-Lookup:** "Find the color with count Y" (Reverse Dictionary Search)
- **Boolean Logic:** "Is Blue Count == Red Count?" (True/False Evaluation)

---

## 🎮 Logic & Algorithm Visualization

### **1. The Profiler: Memory Optimization**
<img width="925" height="775" alt="Image" src="https://github.com/user-attachments/assets/c74ca7a1-7697-49a2-8c87-e9e7cfa0d895" />

**46.8% Reduction in GC Allocation**
Snapshot of the Unity Profiler validating my custom memory management strategy. By implementing component caching and optimizing string concatenation in the render loop, I reduced Garbage Collection (GC) spikes to near zero, ensuring a stable **16ms (60 FPS)** frame time.

---

### **2. Recursive Base State (3x3)**
<img width="414" height="736" alt="Image" src="https://github.com/user-attachments/assets/9291a474-61db-4d77-b62f-9d15baadd9d5" />

**Frequency Analysis Algorithm**
The initialization of the recursive algorithm. The system generates a base **3x3 matrix** and executes a frequency count to validate user input. This sets the baseline for the scaling logic.

---

### **3. Spatial Subdivision (6x6)**
<img width="414" height="736" alt="Image" src="https://github.com/user-attachments/assets/3bab19ca-6d1f-4e1d-b11c-2e03855f8072" />

**Inverse Key-Value Lookup**
The algorithm has recursively subdivided the grid into a **6x6 matrix**. The logic engine switches to **Inverse Lookup mode**: instead of counting colors, it performs a reverse dictionary search (finding the Key associated with Value 4).

---

### **4. High-Density Recursion (12x12)**
<img width="414" height="736" alt="Image" src="https://github.com/user-attachments/assets/7ea5cbf9-12c3-4807-b2b7-e8e23c554124" />

**Boolean Logic Evaluation**
The peak of the recursive algorithm. The grid has subdivided into a high-density matrix. The logic engine shifts to **Boolean Evaluation**, comparing two distinct frequency distributions in real-time to return a True/False validity state.

---

### **5. State Management & Complexity**
<img width="414" height="736" alt="Image" src="https://github.com/user-attachments/assets/85a5fe0b-fcbc-4b5a-80d3-7bf2269ecdae" />

**Constraints & Complexity**
The application manages strict state transitions (Play → Fail) based on time complexity constraints. As the grid subdivides, the search space grows quadratically (O(N^2)), requiring optimized input validation loops to prevent frame drops during rapid state changes.
---

### **6. Full SDLC & Production Deployment**
<img width="1171" height="1142" alt="Image" src="https://github.com/user-attachments/assets/25913162-76fb-4c43-8ce6-f23f35ed83a0" />

Evidence of the complete Software Development Life Cycle (SDLC). I independently managed the build pipeline, compliance, and App Store Optimization (ASO) for both iOS and Android, successfully shipping the application to a global audience with 2,172+ organic users.

---

## 📊 Technical Stats

- **Memory Optimization:** Reduced GC Allocation by **46.8%**.
- **User Base:** **2,172+** Organic Installs.
- **Frame Rate:** Stable **60 FPS** on low-end Android devices.

---

## 👤 Author
**Thanitsak Leuangsupornpong**
Software Engineer (Independent)
