// SPDX-License-Identifier: UNLICENSED
pragma solidity ^0.8.10;

import "@openzeppelin/contracts/access/Ownable.sol";

contract ORTBet is Ownable {
    mapping(bytes32 => Game) public games;
    mapping(bytes32 => FinishedGame) public finishedGames;
    mapping(address => uint256) public userStakes;

    event GameCreated(
        bytes32 indexed gameId,
        address indexed player1,
        uint256 betAmount
    );
    event GameCanceled(bytes32 indexed gameId);
    event GameJoined(bytes32 indexed gameId, address indexed player2);
    event GameStakesDeposited(
        bytes32 indexed gameId,
        address indexed playerAddress,
        bool isDepositerPlayer1
    );
    event ChangeReturned(
        bytes32 indexed gameId,
        address indexed playerAddress,
        uint256 changeAmount
    );
    event GameEnded(bytes32 indexed gameId, GameResult result);
    event StakesWithdrawn(address indexed playerAddress, uint256 amount);

    constructor() Ownable(msg.sender) {}

    function createPendingGame(
        string calldata _gameId,
        address _player1Address,
        uint256 _betAmount
    ) public onlyOwner {
        bytes32 gameIdHash = keccak256(abi.encodePacked(_gameId));
        require(
            !isGameInitiated(gameIdHash) && !isGameFinished(gameIdHash),
            "GameId already taken."
        );
        require(_player1Address != address(0), "Invalid _player1Address.");
        require(_betAmount > 0, "_betAmount must be greater than 0.");

        games[gameIdHash] = Game({
            betAmount: _betAmount,
            player1Address: _player1Address,
            player2Address: address(0),
            started: false,
            player1Paid: false,
            player2Paid: false
        });
        emit GameCreated(gameIdHash, _player1Address, _betAmount);
    }

    function cancelPendingGame(
        bytes32 _gameId
    ) external onlyOwner validGameId(_gameId) gameNotStarted(_gameId) {
        if (games[_gameId].player1Paid) {
            userStakes[games[_gameId].player1Address] += games[_gameId]
                .betAmount;
        }
        if (games[_gameId].player2Paid) {
            userStakes[games[_gameId].player2Address] += games[_gameId]
                .betAmount;
        }

        delete games[_gameId];
        emit GameCanceled(_gameId);
    }

    function joinGame(
        bytes32 _gameId,
        address _player2Address
    ) external onlyOwner validGameId(_gameId) gameNotStarted(_gameId) {
        require(
            games[_gameId].player2Address == address(0),
            "Player2 already specified."
        );
        require(
            games[_gameId].player1Address != _player2Address,
            "That address is already registered as player1."
        );

        games[_gameId].player2Address = _player2Address;
        emit GameJoined(_gameId, _player2Address);
    }

    function depositBetAmount(
        bytes32 _gameId
    ) external payable validGameId(_gameId) gameNotStarted(_gameId) {
        require(
            msg.value >= games[_gameId].betAmount,
            "The amount sent is not sufficient for the bet specified on this game."
        );
        require(
            msg.sender == games[_gameId].player1Address ||
                msg.sender == games[_gameId].player2Address,
            "The sender address is not recognized as a player."
        );

        if (games[_gameId].player1Address == msg.sender) {
            require(
                !games[_gameId].player1Paid,
                "Player1 has already paid their stake."
            );
            games[_gameId].player1Paid = true;
        } else {
            require(
                !games[_gameId].player2Paid,
                "Player2 has already paid their stake."
            );
            games[_gameId].player2Paid = true;
        }

        if (msg.value > games[_gameId].betAmount) {
            uint256 change = msg.value - games[_gameId].betAmount;
            payable(msg.sender).transfer(change);
            emit ChangeReturned(_gameId, msg.sender, change);
        }

        if (games[_gameId].player1Paid && games[_gameId].player2Paid) {
            games[_gameId].started = true;
        }

        emit GameStakesDeposited(
            _gameId,
            msg.sender,
            msg.sender == games[_gameId].player1Address
        );
    }

    function endGame(
        bytes32 _gameId,
        GameResult _gameResult
    ) external onlyOwner validGameId(_gameId) {
        require(
            games[_gameId].started,
            "That game hasn't been started (The players didn't pay their stakes)."
        );

        if (_gameResult == GameResult.Player1Won) {
            userStakes[games[_gameId].player1Address] += games[_gameId]
                .betAmount;
        } else if (_gameResult == GameResult.Player2Won) {
            userStakes[games[_gameId].player2Address] += games[_gameId]
                .betAmount;
        } else {
            userStakes[games[_gameId].player1Address] += games[_gameId]
                .betAmount;
            userStakes[games[_gameId].player2Address] += games[_gameId]
                .betAmount;
        }

        finishedGames[_gameId] = FinishedGame(
            games[_gameId].betAmount,
            games[_gameId].player1Address,
            games[_gameId].player2Address,
            _gameResult
        );
        delete games[_gameId];

        emit GameEnded(_gameId, _gameResult);
    }

    function withdrawStakes(uint256 _amount) external {
        require(_amount > 0, "Withdrawal amount must be greater than 0.");
        require(
            userStakes[msg.sender] >= _amount,
            "Insufficient funds for withdrawal."
        );

        userStakes[msg.sender] -= _amount;
        payable(msg.sender).transfer(_amount);
        emit StakesWithdrawn(msg.sender, _amount);
    }

    receive() external payable {
        revert("Direct deposits not allowed.");
    }

    modifier validGameId(bytes32 _gameId) {
        require(
            isGameInitiated(_gameId) && !isGameFinished(_gameId),
            "Game either not initiated or already finished."
        );
        _;
    }

    modifier gameNotStarted(bytes32 _gameId) {
        require(
            !games[_gameId].started,
            "Invalid action! Game already started."
        );
        _;
    }

    function isGameInitiated(bytes32 _gameId) internal view returns (bool) {
        return games[_gameId].player1Address != address(0);
    }

    function isGameFinished(bytes32 _gameId) internal view returns (bool) {
        return finishedGames[_gameId].player1Address != address(0);
    }

    enum GameResult {
        Draw,
        Player1Won,
        Player2Won
    }

    struct Game {
        uint256 betAmount;
        address player1Address;
        address player2Address;
        bool started;
        bool player1Paid;
        bool player2Paid;
    }

    struct FinishedGame {
        uint256 betAmount;
        address player1Address;
        address player2Address;
        GameResult result;
    }
}
