# Machine Minds

Machine Minds is a 2D action tank game built in Unity that experiments with machine-learning–driven adaptive difficulty. As players progress through the game, the difficulty dynamically adjusts based on their performance and feedback, creating a personalized gameplay experience.

This project was developed by vgbStudios as a university capstone project.

**Platforms:** PC (Windows / macOS), WebGL  
**Core Focus:** Gameplay + Machine Learning  
**Engine:** Unity (C#) + Python (scikit-learn)

---

## Team

- **Gabriel Castejon** — Scrum Master / Integrations  
- **Vladimir** — Back-End / AI  
- **Benjamin** — Front-End / Project Manager  
- **Advisor:** Rachel Fraizer

**Final Presentation:**  
https://www.youtube.com/watch?v=VX-DNo4qVTQ

---

## Game Overview

Machine Minds is a level-based 2D tank shooter where players fight through 20 progressively harder levels across multiple environments:

- Base  
- Desert  
- Swamp  
- Snow  
- Corrupt  

Each level challenges the player with limited ammo, three lives, enemy AI, and terrain effects such as ice and mud that affect movement. A checkpoint system every five levels allows partial score retention, balancing challenge and accessibility.

---

## Core Gameplay Mechanics

### Movement and Boost
- Standard tank movement  
- Boost ability (4x speed, limited duration and fuel)

### Combat
- Limited ammunition per level (10 bullets)  
- Emphasis on accuracy and resource management

### Survivability
- Three lives per run  
- Checkpoints every five levels

### Scoring
- Performance-based scoring  
- Global leaderboards using Firebase

---

## Adaptive Difficulty and Machine Learning

Machine Minds features a machine-learning–powered difficulty adjustment system.

During gameplay, performance metrics such as accuracy, damage taken, survival time, and score progression are collected. Between levels, players complete a short difficulty survey. This data is used to train a supervised learning model (logistic regression) built in Python using scikit-learn.

The trained model dynamically adjusts difficulty by approximately 20% per level, creating a tailored experience for each player.

---

## Development Process

### Game Development
- Built in Unity using C#  
- Component-based architecture  
- 2D physics and optimized rendering with URP  

### Machine Learning
- Python backend using scikit-learn  
- Real player data collected during demo playtests  
- Training data exported to CSV and stored in Firebase  
- Model outputs difficulty scaling parameters  

### Backend and Data
- Firebase Firestore for leaderboards, player stats, and analytics  
- REST API for Python–Unity communication on non-WebGL platforms  
- Local save system using PlayerPrefs  

---

## Platform Support

### WebGL
- Browser-playable build  
- JavaScript interface for ML communication  

### Desktop (Windows / macOS)
- REST API backend  
- Local and cloud save support  

---

## Testing and Feedback

- Unit and integration testing using Unity Test Runner  
- Cross-platform performance testing  
- Over 500 gameplay metric samples collected  
- User surveys and in-person playtesting  
- Iterative balancing based on analytics and feedback  

---

## Technologies Used

- Unity (C#)  
- Python  
- scikit-learn  
- Firebase Firestore  
- REST APIs  
- WebGL  
- Git and GitHub  

---

## Acknowledgements

Special thanks to:
- Rachel Fraizer for project guidance  
- All playtesters who provided gameplay data and feedback, which made the adaptive difficulty system possible
