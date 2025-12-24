using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TicTacToeGame : MonoBehaviour
{
    public enum Difficulty { Easy, Medium, Hard }
    public enum Player { None, Human, AI }
    public enum Symbol { X, O }

    [Header("UI References")]
    public GameObject startGamePanel; // Panel with title and start button
    public GameObject difficultyPanel; // Panel with difficulty selection
    public GameObject gamePanel; // Panel with game board

    public Button startButton; // START GAME button
    public Button easyButton; // Easy difficulty button
    public Button mediumButton; // Medium difficulty button
    public Button hardButton; // Hard difficulty button
    public Button backButton; // Back button to return to start panel
    public Button backFromDifficultyButton; // Back button on difficulty panel

    public Button[] cells;
    public TextMeshProUGUI statusText;
    public Button resetButton;

    [Header("Score Display")]
    public TextMeshProUGUI xScoreText;
    public TextMeshProUGUI oScoreText;

    [Header("Game Settings")]
    public Difficulty currentDifficulty = Difficulty.Medium;
    public Symbol playerSymbol = Symbol.X;

    [Header("Symbol Images")]
    public Sprite xSprite;
    public Sprite oSprite;
    [Header("Symbol Image Size")]
    public float symbolWidth = 80f; // Width of X and O images
    public float symbolHeight = 80f; // Height of X and O images

    [Header("Win Line Settings")]
    public GameObject[] winLines; // 8 pre-created win lines (3 rows, 3 cols, 2 diagonals)

    private Player[,] board = new Player[3, 3];
    private Player currentPlayer = Player.Human;
    private bool gameOver = false;
    private bool gameStarted = false;

    // Score tracking
    private int xScore = 0;
    private int oScore = 0;

    void Start()
    {
        if (cells == null || cells.Length != 9)
        {
            Debug.LogError("TicTacToeGame: Please assign 9 buttons to the 'cells' array in the Inspector!");
            return;
        }

        if (xSprite == null || oSprite == null)
        {
            Debug.LogError("TicTacToeGame: Please assign X and O sprite images in the Inspector!");
            return;
        }

        InitializeGame();
        SetupButtons();
        HideAllWinLines();
        UpdateScoreDisplay();
        ShowStartOverlay();
    }

    void HideAllWinLines()
    {
        if (winLines != null)
        {
            foreach (GameObject line in winLines)
            {
                if (line != null)
                {
                    line.SetActive(false);
                }
            }
        }
    }

    void SetupButtons()
    {
        if (cells == null || cells.Length == 0)
            return;

        for (int i = 0; i < cells.Length; i++)
        {
            int index = i;
            if (cells[i] != null)
            {
                cells[i].onClick.AddListener(() => OnCellClicked(index));
            }
        }

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetGame);
        }

        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(BackToStartPanel);
        }

        if (backFromDifficultyButton != null)
        {
            backFromDifficultyButton.onClick.AddListener(BackToStartPanelFromDifficulty);
        }

        if (easyButton != null)
        {
            easyButton.onClick.AddListener(() => SelectDifficulty(Difficulty.Easy));
        }

        if (mediumButton != null)
        {
            mediumButton.onClick.AddListener(() => SelectDifficulty(Difficulty.Medium));
        }

        if (hardButton != null)
        {
            hardButton.onClick.AddListener(() => SelectDifficulty(Difficulty.Hard));
        }
    }

    void InitializeGame()
    {
        board = new Player[3, 3];
        currentPlayer = Player.Human;
        gameOver = false;

        if (cells == null || cells.Length == 0)
        {
            Debug.LogError("TicTacToeGame: Cells array is not assigned!");
            return;
        }

        foreach (Button cell in cells)
        {
            if (cell != null)
            {
                // Try to find child Symbol image first
                Transform symbolTransform = cell.transform.Find("Symbol");
                Image cellImage;

                if (symbolTransform != null)
                {
                    cellImage = symbolTransform.GetComponent<Image>();
                }
                else
                {
                    cellImage = cell.GetComponent<Image>();
                }

                if (cellImage != null)
                {
                    cellImage.sprite = null;
                    cellImage.color = new Color(1, 1, 1, 0); // Make transparent
                }
                cell.interactable = true;
            }
        }

        // Hide all win lines
        HideAllWinLines();

        string playerSymbolText = playerSymbol == Symbol.X ? "X" : "O";
        string aiSymbolText = playerSymbol == Symbol.X ? "O" : "X";
        UpdateStatus($"Your turn! You are {playerSymbolText}");
    }

    void OnCellClicked(int index)
    {
        if (gameOver || currentPlayer != Player.Human || !gameStarted)
            return;

        int row = index / 3;
        int col = index % 3;

        if (board[row, col] == Player.None)
        {
            MakeMove(row, col, Player.Human);

            if (!gameOver)
            {
                currentPlayer = Player.AI;
                UpdateStatus("AI is thinking...");
                StartCoroutine(AITurnDelayed());
            }
        }
    }

    IEnumerator AITurnDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        AITurn();
    }

    void AITurn()
    {
        if (gameOver)
            return;

        Vector2Int move = GetAIMove();
        MakeMove(move.x, move.y, Player.AI);

        if (!gameOver)
        {
            currentPlayer = Player.Human;
            string playerSymbolText = playerSymbol == Symbol.X ? "X" : "O";
            UpdateStatus($"Your turn! You are {playerSymbolText}");
        }
    }

    Vector2Int GetAIMove()
    {
        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                return GetEasyMove();
            case Difficulty.Medium:
                return GetMediumMove();
            case Difficulty.Hard:
                return GetHardMove();
            default:
                return GetHardMove();
        }
    }

    // Easy: Random valid move
    Vector2Int GetEasyMove()
    {
        List<Vector2Int> availableMoves = GetAvailableMoves();
        return availableMoves[Random.Range(0, availableMoves.Count)];
    }

    // Medium: 50% chance of best move, 50% random
    Vector2Int GetMediumMove()
    {
        if (Random.value < 0.5f)
        {
            return GetHardMove();
        }
        else
        {
            return GetEasyMove();
        }
    }

    // Hard: Always uses Minimax (impossible to beat)
    Vector2Int GetHardMove()
    {
        int bestScore = int.MinValue;
        Vector2Int bestMove = new Vector2Int(0, 0);

        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if (board[row, col] == Player.None)
                {
                    board[row, col] = Player.AI;
                    int score = Minimax(board, 0, false);
                    board[row, col] = Player.None;

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = new Vector2Int(row, col);
                    }
                }
            }
        }

        return bestMove;
    }

    int Minimax(Player[,] boardState, int depth, bool isMaximizing)
    {
        Player winner = CheckWinner();

        if (winner == Player.AI)
            return 10 - depth;
        if (winner == Player.Human)
            return depth - 10;
        if (IsBoardFull())
            return 0;

        if (isMaximizing)
        {
            int bestScore = int.MinValue;
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (boardState[row, col] == Player.None)
                    {
                        boardState[row, col] = Player.AI;
                        int score = Minimax(boardState, depth + 1, false);
                        boardState[row, col] = Player.None;
                        bestScore = Mathf.Max(score, bestScore);
                    }
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (boardState[row, col] == Player.None)
                    {
                        boardState[row, col] = Player.Human;
                        int score = Minimax(boardState, depth + 1, true);
                        boardState[row, col] = Player.None;
                        bestScore = Mathf.Min(score, bestScore);
                    }
                }
            }
            return bestScore;
        }
    }

    void MakeMove(int row, int col, Player player)
    {
        board[row, col] = player;
        int index = row * 3 + col;

        if (cells != null && index < cells.Length && cells[index] != null)
        {
            // Determine which sprite to use based on player and player's chosen symbol
            Sprite spriteToUse;
            if (player == Player.Human)
            {
                spriteToUse = playerSymbol == Symbol.X ? xSprite : oSprite;
            }
            else
            {
                spriteToUse = playerSymbol == Symbol.X ? oSprite : xSprite;
            }

            // Try to find a child Image component named "Symbol" for better size control
            Transform symbolTransform = cells[index].transform.Find("Symbol");
            Image cellImage = null;

            if (symbolTransform != null)
            {
                // Use child Image if it exists
                cellImage = symbolTransform.GetComponent<Image>();
            }
            else
            {
                // Fallback to button's own Image component
                cellImage = cells[index].GetComponent<Image>();
            }

            if (cellImage != null)
            {
                cellImage.sprite = spriteToUse;
                cellImage.color = Color.white; // Make visible

                // Apply size if using child image
                if (symbolTransform != null)
                {
                    RectTransform rectTransform = symbolTransform.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        rectTransform.sizeDelta = new Vector2(symbolWidth, symbolHeight);
                    }
                }
            }
            cells[index].interactable = false;
        }

        CheckGameEnd();
    }

    void CheckGameEnd()
    {
        int[] winningLine = GetWinningLine();
        string playerSymbolText = playerSymbol == Symbol.X ? "X" : "O";
        string aiSymbolText = playerSymbol == Symbol.X ? "O" : "X";

        if (winningLine != null)
        {
            Player winner = board[winningLine[0] / 3, winningLine[0] % 3];

            if (winner == Player.Human)
            {
                // Increment score based on player's symbol
                if (playerSymbol == Symbol.X)
                {
                    xScore++;
                }
                else
                {
                    oScore++;
                }
                UpdateScoreDisplay();
                UpdateStatus($"You Win! ({playerSymbolText} wins)");
                gameOver = true;
                DrawWinLine(winningLine);
                DisableAllCells();
            }
            else if (winner == Player.AI)
            {
                // Increment score based on AI's symbol
                if (playerSymbol == Symbol.X)
                {
                    oScore++;
                }
                else
                {
                    xScore++;
                }
                UpdateScoreDisplay();
                UpdateStatus($"AI Wins! ({aiSymbolText} wins)");
                gameOver = true;
                DrawWinLine(winningLine);
                DisableAllCells();
            }
        }
        else if (IsBoardFull())
        {
            UpdateStatus("It's a Draw!");
            gameOver = true;
        }
    }

    int[] GetWinningLine()
    {
        // Check rows
        for (int row = 0; row < 3; row++)
        {
            if (board[row, 0] != Player.None &&
                board[row, 0] == board[row, 1] &&
                board[row, 1] == board[row, 2])
            {
                return new int[] { row * 3 + 0, row * 3 + 1, row * 3 + 2 };
            }
        }

        // Check columns
        for (int col = 0; col < 3; col++)
        {
            if (board[0, col] != Player.None &&
                board[0, col] == board[1, col] &&
                board[1, col] == board[2, col])
            {
                return new int[] { 0 * 3 + col, 1 * 3 + col, 2 * 3 + col };
            }
        }

        // Check diagonal (top-left to bottom-right)
        if (board[0, 0] != Player.None &&
            board[0, 0] == board[1, 1] &&
            board[1, 1] == board[2, 2])
        {
            return new int[] { 0, 4, 8 };
        }

        // Check diagonal (top-right to bottom-left)
        if (board[0, 2] != Player.None &&
            board[0, 2] == board[1, 1] &&
            board[1, 1] == board[2, 0])
        {
            return new int[] { 2, 4, 6 };
        }

        return null;
    }

    Player CheckWinner()
    {
        // Check rows
        for (int row = 0; row < 3; row++)
        {
            if (board[row, 0] != Player.None &&
                board[row, 0] == board[row, 1] &&
                board[row, 1] == board[row, 2])
            {
                return board[row, 0];
            }
        }

        // Check columns
        for (int col = 0; col < 3; col++)
        {
            if (board[0, col] != Player.None &&
                board[0, col] == board[1, col] &&
                board[1, col] == board[2, col])
            {
                return board[0, col];
            }
        }

        // Check diagonals
        if (board[0, 0] != Player.None &&
            board[0, 0] == board[1, 1] &&
            board[1, 1] == board[2, 2])
        {
            return board[0, 0];
        }

        if (board[0, 2] != Player.None &&
            board[0, 2] == board[1, 1] &&
            board[1, 1] == board[2, 0])
        {
            return board[0, 2];
        }

        return Player.None;
    }

    void DrawWinLine(int[] winningCells)
    {
        if (winLines == null || winLines.Length != 8 || winningCells == null || winningCells.Length != 3)
            return;

        // Sort the winning cells to ensure consistent checking
        System.Array.Sort(winningCells);

        // Determine which line to show based on winning cells
        // Lines order: Row0, Row1, Row2, Col0, Col1, Col2, Diag1(\), Diag2(/)
        int lineIndex = -1;

        // Check rows (sorted order)
        if (winningCells[0] == 0 && winningCells[1] == 1 && winningCells[2] == 2) lineIndex = 0; // Row 0
        else if (winningCells[0] == 3 && winningCells[1] == 4 && winningCells[2] == 5) lineIndex = 1; // Row 1
        else if (winningCells[0] == 6 && winningCells[1] == 7 && winningCells[2] == 8) lineIndex = 2; // Row 2
        // Check columns (sorted order)
        else if (winningCells[0] == 0 && winningCells[1] == 3 && winningCells[2] == 6) lineIndex = 3; // Col 0
        else if (winningCells[0] == 1 && winningCells[1] == 4 && winningCells[2] == 7) lineIndex = 4; // Col 1
        else if (winningCells[0] == 2 && winningCells[1] == 5 && winningCells[2] == 8) lineIndex = 5; // Col 2
        // Check diagonals (sorted order)
        else if (winningCells[0] == 0 && winningCells[1] == 4 && winningCells[2] == 8) lineIndex = 6; // Diagonal \
        else if (winningCells[0] == 2 && winningCells[1] == 4 && winningCells[2] == 6) lineIndex = 7; // Diagonal /

        string lineName = lineIndex switch
        {
            0 => "Row 0 (top)",
            1 => "Row 1 (middle)",
            2 => "Row 2 (bottom)",
            3 => "Col 0 (left)",
            4 => "Col 1 (middle)",
            5 => "Col 2 (right)",
            6 => "Diagonal \\",
            7 => "Diagonal /",
            _ => "Unknown"
        };

        Debug.Log($"Winning cells: [{winningCells[0]}, {winningCells[1]}, {winningCells[2]}], Line index: {lineIndex} ({lineName})");

        // Show the winning line
        if (lineIndex >= 0 && lineIndex < winLines.Length && winLines[lineIndex] != null)
        {
            winLines[lineIndex].SetActive(true);
            Debug.Log($"Activated win line {lineIndex}: {winLines[lineIndex].name}");
        }
        else
        {
            Debug.LogWarning($"Could not find matching win line for cells [{winningCells[0]}, {winningCells[1]}, {winningCells[2]}]");
        }
    }

    bool IsBoardFull()
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if (board[row, col] == Player.None)
                    return false;
            }
        }
        return true;
    }

    List<Vector2Int> GetAvailableMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if (board[row, col] == Player.None)
                    moves.Add(new Vector2Int(row, col));
            }
        }
        return moves;
    }

    void DisableAllCells()
    {
        if (cells == null)
            return;

        foreach (Button cell in cells)
        {
            if (cell != null)
            {
                cell.interactable = false;
            }
        }
    }

    void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    void UpdateScoreDisplay()
    {
        if (xScoreText != null)
        {
            xScoreText.text = $"X: {xScore}";
        }

        if (oScoreText != null)
        {
            oScoreText.text = $"O: {oScore}";
        }
    }

    void ResetGame()
    {
        InitializeGame();
    }

    void ShowStartOverlay()
    {
        // Show start panel, hide others
        if (startGamePanel != null)
        {
            startGamePanel.SetActive(true);
        }

        if (difficultyPanel != null)
        {
            difficultyPanel.SetActive(false);
        }

        if (gamePanel != null)
        {
            gamePanel.SetActive(false);
        }

        gameStarted = false;
    }

    public void StartGame()
    {
        // Hide start panel, show difficulty panel
        if (startGamePanel != null)
        {
            startGamePanel.SetActive(false);
        }

        if (difficultyPanel != null)
        {
            difficultyPanel.SetActive(true);
        }

        if (gamePanel != null)
        {
            gamePanel.SetActive(false);
        }
    }

    public void SelectDifficulty(Difficulty difficulty)
    {
        // Set the selected difficulty
        currentDifficulty = difficulty;

        // Hide difficulty panel, show game panel
        if (difficultyPanel != null)
        {
            difficultyPanel.SetActive(false);
        }

        if (gamePanel != null)
        {
            gamePanel.SetActive(true);
        }

        gameStarted = true;

        // Initialize the game board
        InitializeGame();

        // Show initial status
        string playerSymbolText = playerSymbol == Symbol.X ? "X" : "O";
        UpdateStatus($"Your turn! You are {playerSymbolText}");
    }

    public void BackToStartPanel()
    {
        // Show difficulty panel, hide game panel
        if (startGamePanel != null)
        {
            startGamePanel.SetActive(false);
        }

        if (difficultyPanel != null)
        {
            difficultyPanel.SetActive(true);
        }

        if (gamePanel != null)
        {
            gamePanel.SetActive(false);
        }

        gameStarted = false;

        // Reset the game state
        InitializeGame();
    }

    public void BackToStartPanelFromDifficulty()
    {
        // Show start panel, hide difficulty panel
        if (startGamePanel != null)
        {
            startGamePanel.SetActive(true);
        }

        if (difficultyPanel != null)
        {
            difficultyPanel.SetActive(false);
        }

        if (gamePanel != null)
        {
            gamePanel.SetActive(false);
        }

        gameStarted = false;
    }
}
