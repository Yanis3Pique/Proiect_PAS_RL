# ⚽ Multi-Agent Reinforcement Learning — 2v2 Volta Soccer (Unity ML-Agents)

This project implements a **multi-agent reinforcement learning environment** in Unity, where **two teams of two agents** learn to play a simplified Volta-style soccer game.

The agents are not scripted.

They **learn from scratch** through self-play by interacting with the environment, pushing the ball, scoring goals, and optimizing team-based rewards.

---

## 🎮 Environment Overview

- **Game Mode:** 2 vs 2
- **Engine:** Unity
- **RL Framework:** Unity ML-Agents
- **Physics:** Rigidbody-based interactions
- **Episode Structure:** Timed match (90 seconds default)
- **Control Type:** Continuous actions (movement + rotation)

The ball is fully physics-driven.  
Players must learn positioning, rotation, and coordinated pushing behavior to score.

---

## 🧠 Learning Setup

### Multi-Agent Training

Each team is registered as a `SimpleMultiAgentGroup`:

- Team rewards are shared.
- Episodes end when:
  - A goal is scored
  - The timer expires

This encourages **cooperation within teams** and **competition between teams**.

---

## 🔍 Observation Space

Each agent observes:

- Direction to the ball (normalized vector)
- Distance to the ball
- Own team ID
- Relative position to own goal
- Relative position to opponent goal
- Normalized score difference

This allows agents to:
- Understand spatial relationships
- Track match progress
- Adapt strategy based on score state

---

## 🎮 Action Space (Continuous)

Each agent outputs:

| Action | Description |
|--------|------------|
| `moveZ` | Forward / backward movement |
| `moveX` | Side movement |
| `rotate` | Rotation |

Movement is applied via Rigidbody physics.

---

## 🏆 Reward System

### Group Rewards

- +1.0 → Team scores a goal
- -1.0 → Team concedes
- -0.75 → Ball remains stuck (anti-stalling penalty)

### Individual Adjustment

If a player is the last one who touched the ball before conceding:
- Additional negative reward applied

This discourages reckless defensive touches.

---

## ⏱ Game Flow

Each episode:

1. Timer starts (default: 90 seconds)
2. Agents interact in real-time
3. Goals immediately end the episode
4. Groups receive rewards
5. Environment resets

If the ball becomes stationary for too long:
- Both teams receive a penalty

This prevents degenerate "do nothing" policies.

---

## 🧩 Key Components

### `PlayerAgent.cs`

- Handles observations
- Executes movement and rotation
- Receives rewards
- Supports heuristic keyboard control (for debugging)

### `GameManager.cs`

- Handles scoring
- Controls timer
- Applies group rewards
- Detects ball stalling
- Resets environment

### `BallDetector.cs`

- Detects goals via collision
- Triggers reward logic

---

## 🏗 Training Characteristics

- Fully physics-based interaction
- Competitive self-play
- Cooperative team learning
- Sparse but meaningful rewards
- Anti-stall shaping

This setup encourages emergent behaviors such as:
- Positioning
- Defensive blocking
- Offensive pushing
- Goal awareness

---

## 🧪 Heuristic Mode

For testing, agents can be manually controlled:

| Key | Action |
|-----|--------|
| W/S | Forward / Back |
| A/D | Strafe |
| Q/E | Rotate |

---

## 🚀 Why This Project Is Interesting

This project explores:

- Multi-agent cooperation and competition
- Reward shaping in adversarial settings
- Physics-aware RL policies
- Self-play dynamics
- Team-level credit assignment

The agents are not programmed with soccer logic.

They discover strategy through reward optimization.

---

## 📈 Possible Extensions

- Curriculum learning (1v1 → 2v2)
- Larger field variations
- Goalkeeper specialization
- Communication channels between agents
- PPO hyperparameter tuning experiments
- League training

---

## 🧑‍💻 Technologies Used

- Unity (C#)
- Unity ML-Agents Toolkit
- ONNX model export
- Rigidbody physics
- TextMeshPro (UI)

---

## 📌 Final Note

This environment demonstrates how relatively simple reward structures and observations can lead to complex emergent team behaviors.

The agents learn to:
- Chase the ball
- Protect their goal
- Coordinate implicitly
- Optimize team outcomes

No scripted soccer logic.
Only reinforcement learning.

---

If you found this project interesting, feel free to explore the training configuration or extend the environment.⚽🤖
