"""Generate original, deterministic 16-bit PCM music and effects for the first slice.

No samples, soundfonts, or third-party recordings are used. Run from repository root.
"""

from math import exp, pi, sin
from pathlib import Path
import random
import struct
import wave

RATE = 22050
ROOT = Path(__file__).resolve().parents[1] / "client/godot/audio"


def write(name: str, samples: list[float]) -> None:
    target = ROOT / name
    target.parent.mkdir(parents=True, exist_ok=True)
    peak = max(1.0, max(abs(value) for value in samples))
    with wave.open(str(target), "wb") as output:
        output.setnchannels(1)
        output.setsampwidth(2)
        output.setframerate(RATE)
        output.writeframes(b"".join(struct.pack("<h", int(max(-1, min(1, value / peak)) * 26000)) for value in samples))


def tone(freq: float, seconds: float, color: float = 0.3) -> list[float]:
    count = int(seconds * RATE)
    return [
        (sin(2 * pi * freq * i / RATE) + color * sin(4 * pi * freq * i / RATE))
        * min(1.0, i / (RATE * 0.08), (count - i) / (RATE * 0.18))
        for i in range(count)
    ]


def compose(name: str, roots: list[int], tempo: float, combat: bool = False) -> None:
    beat = 60 / tempo
    duration = len(roots) * 4 * beat
    count = int(duration * RATE)
    samples = [0.0] * count
    rng = random.Random(430 if combat else 172)
    for bar, root in enumerate(roots):
        start = int(bar * 4 * beat * RATE)
        chord = [root, root + 3, root + 7] if combat else [root, root + 5, root + 7]
        for note in chord:
            frequency = 440 * 2 ** ((note - 69) / 12)
            for i in range(int(4 * beat * RATE)):
                at = start + i
                if at >= count:
                    break
                swell = sin(pi * i / (4 * beat * RATE)) ** 2
                samples[at] += 0.10 * swell * (sin(2 * pi * frequency * i / RATE) + 0.22 * sin(4 * pi * frequency * i / RATE))
        bass = 440 * 2 ** ((root - 24 - 69) / 12)
        for i in range(int(4 * beat * RATE)):
            at = start + i
            if at < count:
                samples[at] += 0.14 * sin(pi * i / (4 * beat * RATE)) ** 2 * sin(2 * pi * bass * i / RATE)
        for step in range(8):
            note = chord[(step + bar) % 3] + (12 if combat else 0)
            frequency = 440 * 2 ** ((note - 69) / 12)
            attack = start + int(step * beat / 2 * RATE)
            length = int(beat * (0.27 if combat else 0.34) * RATE)
            for i in range(length):
                at = attack + i
                if at < count:
                    samples[at] += (0.10 if combat else 0.065) * exp(-5 * i / length) * sin(2 * pi * frequency * i / RATE)
        if combat:
            for step in range(4):
                attack = start + int(step * beat * RATE)
                for i in range(int(0.13 * RATE)):
                    at = attack + i
                    if at < count:
                        samples[at] += 0.25 * exp(-38 * i / RATE) * (sin(2 * pi * 88 * i / RATE) + 0.13 * rng.uniform(-1, 1))
    # A brief fade at the loop seam avoids clicks when the stream restarts.
    fade = int(0.13 * RATE)
    for i in range(fade):
        samples[i] *= i / fade
        samples[-i - 1] *= i / fade
    write(name, samples)


def effect(name: str, kind: str) -> None:
    seconds = 0.34 if kind == "rift" else 0.22
    rng = random.Random(sum(map(ord, name)))
    samples = []
    for i in range(int(seconds * RATE)):
        t = i / RATE
        fade = exp(-14 * t)
        noise = rng.uniform(-1, 1)
        if kind == "axe": value = fade * (0.4 * noise + sin(2 * pi * (125 - 200 * t) * t))
        elif kind == "impact": value = fade * (0.28 * noise + sin(2 * pi * (75 - 90 * t) * t))
        elif kind == "guard": value = fade * (sin(2 * pi * 410 * t) + 0.3 * sin(2 * pi * 615 * t))
        elif kind == "rift": value = fade * (sin(2 * pi * (250 + 1100 * t) * t) + 0.25 * noise)
        elif kind == "dash": value = fade * (0.6 * noise + sin(2 * pi * 170 * t))
        else: value = fade * (0.5 * noise + sin(2 * pi * 95 * t))
        samples.append(value)
    write(name, samples)


if __name__ == "__main__":
    compose("music/citadel.wav", [45, 41, 43, 40], 72)
    compose("music/forest.wav", [40, 38, 36, 38], 66)
    compose("music/combat.wav", [40, 43, 38, 36], 112, True)
    for label in ("axe", "impact", "guard", "rift", "dash", "hit"):
        effect(f"sfx/{label}.wav", label)
