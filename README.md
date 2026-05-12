# 🎲 Gra w Kości (Yahtzee) — Założenia Projektowe

> Gra planszowa dla 2–4 graczy zaimplementowana w języku **C#**, oparta na zasadach klasycznej gry Yahtzee.

---

## Spis treści

1. [Opis gry](#opis-gry)
2. [Wymagania funkcjonalne](#wymagania-funkcjonalne)
3. [Zasady gry](#zasady-gry)
4. [System punktacji](#system-punktacji)
5. [Architektura projektu](#architektura-projektu)
6. [Technologie i środowisko](#technologie-i-środowisko)
7. [Gracz AI](#gracz-ai)
8. [Interfejs użytkownika](#interfejs-użytkownika)
9. [Warunki zakończenia gry](#warunki-zakończenia-gry)
10. [Roadmapa](#roadmapa)

---

## Opis gry

Gra w kości to turowa gra logiczno-losowa dla **2 do 4 graczy**. Każdy gracz stara się uzyskać jak największą liczbę punktów, rzucając pięcioma klasycznymi sześciennymi kośćmi i dobierając optymalne kategorie punktowania. Gra kończy się, gdy wszyscy gracze zapełnią swoje karty wyników.

---

## Wymagania funkcjonalne

| ID  | Wymaganie |
|-----|-----------|
| F01 | Menu startowe z wyborem liczby graczy (2–4) |
| F02 | Obsługa tury każdego gracza z maksymalnie 3 rzutami |
| F03 | Możliwość zatrzymania wybranych kości między rzutami |
| F04 | Wyświetlanie aktualnego stanu kości po każdym rzucie |
| F05 | Karta wyników z dwiema tabelkami dla każdego gracza |
| F06 | Możliwość wyboru kategorii punktowej po serii rzutów |
| F07 | Walidacja: każda kategoria może być użyta tylko raz |
| F08 | Automatyczne wyliczanie premii za tabelkę 1 (≥63 pkt → +35 pkt) |
| F09 | Wyświetlanie końcowego rankingu po zakończeniu gry |
| F10 | (Opcjonalnie) Gracz sterowany przez AI |

---

## Zasady gry

### Kości

Gra używa **5 klasycznych kości sześciennych** z wartościami od 1 do 6.

### Przebieg tury

```
Rzut 1: Gracz rzuca wszystkimi 5 kośćmi (obowiązkowy)
         ↓
Gracz wybiera, które kości zatrzymać
         ↓
Rzut 2: Gracz rzuca niezatrzymanymi kośćmi (opcjonalny)
         ↓
Gracz wybiera, które kości zatrzymać
         ↓
Rzut 3: Gracz rzuca niezatrzymanymi kośćmi (opcjonalny)
         ↓
Gracz wybiera kategorię z karty wyników → przydzielenie punktów
```

**Ważne zasady:**
- Gracz **zawsze musi** wybrać kategorię po zakończeniu serii rzutów, nawet jeśli kombinacja nie pasuje do żadnej kategorii (wynik = 0 pkt).
- Każdą kategorię można użyć **tylko raz** przez cały czas trwania gry.
- Gracz może zakończyć serię rzutów przed wykonaniem wszystkich 3 (wybrać kategorię po 1. lub 2. rzucie).

---

## System punktacji

### Tabelka 1 — Górna sekcja (Jedności)

Punkty liczone jako **suma wyrzuconych kości** danej wartości.

| Kategoria | Warunek | Przykład (układ: 4, 3, 4, 4, 1) |
|-----------|---------|----------------------------------|
| Jedynki   | Suma wyrzuconych jedynek  | Wynik: 1 pkt (jedna jedynka)   |
| Dwójki    | Suma wyrzuconych dwójek   | Wynik: 0 pkt (brak dwójek)     |
| Trójki    | Suma wyrzuconych trójek   | Wynik: 3 pkt (jedna trójka)    |
| Czwórki   | Suma wyrzuconych czwórek  | Wynik: 12 pkt (trzy czwórki)   |
| Piątki    | Suma wyrzuconych piątek   | Wynik: 0 pkt (brak piątek)     |
| Szóstki   | Suma wyrzuconych szóstek  | Wynik: 0 pkt (brak szóstek)    |

#### 🎁 Premia za Tabelkę 1

Jeśli suma punktów z Tabelki 1 osiągnie **co najmniej 63 punkty**, gracz otrzymuje **jednorazową premię w wysokości 35 punktów**.

> 63 pkt to próg odpowiadający uzyskaniu co najmniej trzech kości każdej wartości w każdej kategorii (3×1 + 3×2 + 3×3 + 3×4 + 3×5 + 3×6 = 63).

---

### Tabelka 2 — Dolna sekcja (Kombinacje)

| Kategoria        | Warunek zdobycia pkt           | Punkty                     |
|-----------------|-------------------------------|---------------------------|
| Trójka           | Co najmniej 3 jednakowe kości | Suma **wszystkich** 5 kości |
| Czwórka          | Co najmniej 4 jednakowe kości | Suma **wszystkich** 5 kości |
| Full             | Trójka + para                 | **25 pkt**                 |
| Mały strit       | 4 kolejne kości (1-2-3-4, 2-3-4-5 lub 3-4-5-6) | **30 pkt** |
| Duży strit       | 5 kolejnych kości (1-2-3-4-5 lub 2-3-4-5-6)    | **40 pkt** |
| Król (Yahtzee)   | 5 jednakowych kości           | **50 pkt**                 |
| Szansa           | Brak (zawsze dostępna)        | Suma **wszystkich** 5 kości |

---

## Architektura projektu

### Proponowana struktura katalogów

```
GraWKosci/
├── GraWKosci.sln
├── GraWKosci/
│   ├── Program.cs                  # Punkt wejścia aplikacji
│   ├── Models/
│   │   ├── Dice.cs                 # Model kości (wartość, czy zatrzymana)
│   │   ├── Player.cs               # Model gracza (nazwa, karta wyników)
│   │   ├── ScoreCard.cs            # Karta wyników gracza (13 kategorii)
│   │   └── GameState.cs            # Globalny stan gry (gracze, tura, runda)
│   ├── Services/
│   │   ├── DiceRoller.cs           # Logika rzucania kośćmi (RNG)
│   │   ├── ScoreCalculator.cs      # Obliczanie punktów dla każdej kategorii
│   │   └── GameEngine.cs           # Główna logika tury i przebiegu gry
│   ├── AI/
│   │   ├── IAiStrategy.cs          # Interfejs strategii AI
│   │   └── OptimalAiStrategy.cs    # Implementacja optymalnej strategii AI
│   └── UI/
│       ├── ConsoleRenderer.cs      # Wyświetlanie stanu gry w konsoli
│       └── InputHandler.cs         # Obsługa wejścia od użytkownika
└── GraWKosci.Tests/
    ├── ScoreCalculatorTests.cs
    ├── GameEngineTests.cs
    └── AiStrategyTests.cs
```

### Kluczowe klasy i odpowiedzialności

#### `Dice`
```csharp
public class Dice
{
    public int Value { get; set; }      // Aktualna wartość kości (1–6)
    public bool IsHeld { get; set; }    // Czy kość jest zatrzymana
}
```

#### `ScoreCard`
```csharp
public class ScoreCard
{
    // Tabelka 1
    public int? Ones { get; set; }
    public int? Twos { get; set; }
    public int? Threes { get; set; }
    public int? Fours { get; set; }
    public int? Fives { get; set; }
    public int? Sixes { get; set; }
    public int? UpperBonus => (UpperSectionTotal >= 63) ? 35 : 0;

    // Tabelka 2
    public int? ThreeOfAKind { get; set; }
    public int? FourOfAKind { get; set; }
    public int? FullHouse { get; set; }
    public int? SmallStraight { get; set; }
    public int? LargeStraight { get; set; }
    public int? Yahtzee { get; set; }
    public int? Chance { get; set; }

    public int TotalScore { get; }
}
```

#### `ScoreCalculator`

Odpowiada za obliczanie potencjalnych punktów dla danego układu kości i każdej kategorii. Metody statyczne, czysto funkcyjne — łatwe do testowania jednostkowego.

#### `GameEngine`

Koordynuje przebieg gry: kolejność graczy, zarządzanie turą, wywołanie rzutów, przyjmowanie wyboru kategorii, sprawdzenie warunków końca gry.

---

## Technologie i środowisko

| Element             | Wartość                          |
|--------------------|----------------------------------|
| Język              | C# 12                            |
| Framework          | .NET 8                           |
| Typ projektu       | Aplikacja konsolowa              |
| Testy jednostkowe  | xUnit + FluentAssertions         |
| IDE                | Visual Studio 2022 / Rider       |
| Wersjonowanie      | Git + GitHub                     |

---

## Gracz AI

> Implementacja planowana jako rozszerzenie po ukończeniu podstawowej wersji gry.

### Cel

Gracz AI podejmuje decyzje (które kości zatrzymać, którą kategorię wybrać) w oparciu o **obliczone optymalne strategie**, bazując na matematycznych oczekiwaniach punktowych.

### Źródła i podejście

- Strategia oparta na analizie z Wikipedii: [Yahtzee — Strategia](https://pl.wikipedia.org/wiki/Yahtzee#Strategia)
- Algorytm podejmowania decyzji:
  1. **Wybór kości do zatrzymania:** dla każdego możliwego podzbioru kości oblicz oczekiwaną wartość punktową (EV) przy pozostałych rzutach, wybierz podzbiór maksymalizujący EV.
  2. **Wybór kategorii:** po ostatnim rzucie wybierz kategorię, która maksymalizuje łączny wynik końcowy, biorąc pod uwagę już zajęte i wolne pola.

### Interfejs AI

```csharp
public interface IAiStrategy
{
    // Zwraca indeksy kości, które AI chce zatrzymać (0–4)
    IEnumerable<int> ChooseDiceToHold(Dice[] dice, ScoreCard scoreCard, int rollsLeft);

    // Zwraca wybraną kategorię do zapisu
    ScoreCategory ChooseCategory(Dice[] dice, ScoreCard scoreCard);
}
```

### Poziomy trudności (opcjonalnie)

| Poziom     | Opis |
|-----------|------|
| Łatwy      | AI losowo wybiera kości i kategorie |
| Średni     | AI stosuje proste heurystyki (np. zawsze kompletuj strit lub króla) |
| Trudny     | AI stosuje pełną optymalną strategię EV |

---

## Interfejs użytkownika

Gra działa w **trybie konsolowym**. Interfejs wyświetla:

- Menu główne z wyborem liczby graczy i trybem gry (PvP / PvAI)
- Stan kości po każdym rzucie (wartości + czy zatrzymana)
- Aktualną kartę wyników gracza z dostępnymi kategoriami
- Komunikaty o wyborach gracza i przydzielonych punktach
- Końcowy ranking wszystkich graczy po zakończeniu gry

**Przykładowy wygląd tury:**

```
=== Tura gracza: Michał | Rzut 1/3 ===
Kości: [4] [3] [4] [4] [1]
       (  ) (  ) (  ) (  ) (  )

Które kości zatrzymać? (np. 1 3 4 lub Enter by rzucić wszystkimi):
> 1 3 4

=== Rzut 2/3 ===
Kości: [4]* [2] [4]* [4]* [6]
       (✓)  (  ) (✓)  (✓)  (  )
```

---

## Warunki zakończenia gry

Gra kończy się, gdy **każdy gracz** wypełni wszystkie 13 kategorii w swojej karcie wyników (6 z Tabelki 1 + 7 z Tabelki 2).

Po zakończeniu gry wyświetlany jest **końcowy ranking** ze szczegółowym podsumowaniem punktacji każdego gracza (Tabelka 1, premia, Tabelka 2, suma końcowa). Wygrywa gracz z **najwyższą łączną liczbą punktów**.

---

## Roadmapa

```
Faza 1 — Podstawy (MVP)
  ✅ Definicja modeli danych (Dice, Player, ScoreCard)
  ✅ Logika rzutania i zatrzymywania kości
  ✅ Obliczanie punktów dla wszystkich 13 kategorii
  ✅ Obsługa tury gracza z 3 rzutami
  ✅ Menu wyboru liczby graczy

Faza 2 — Kompletna rozgrywka
  ⬜ Pełna pętla gry dla 2–4 graczy
  ⬜ Premia za Tabelkę 1
  ⬜ Warunek końca gry + ranking końcowy
  ⬜ Testy jednostkowe

Faza 3 — Gracz AI
  ⬜ Interfejs IAiStrategy
  ⬜ Prosta strategia heurystyczna (poziom łatwy)
  ⬜ Optymalna strategia EV (poziom trudny)
  ⬜ Integracja AI z silnikiem gry

Faza 4 — Polishing
  ⬜ Lepsza obsługa błędów i walidacja wejścia
  ⬜ Kolorowy interfejs konsoli (Spectre.Console)
  ⬜ Zapis i wczytanie stanu gry
```

---

*Projekt stworzony w celach edukacyjnych. Zasady gry oparte na klasycznej grze Yahtzee.*