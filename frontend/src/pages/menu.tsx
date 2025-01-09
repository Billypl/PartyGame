import { URL } from "node:url";
import React from "react";
import { useNavigate } from "react-router-dom";
import { useGameContext } from "../hooks/useGameContext";

import styles from "../styles/Menu.module.scss";

const Menu: React.FC = () => {
  const navigate = useNavigate();
  const { nickname, setNickname, difficulty, setDifficulty } = useGameContext();

  const startGame = () => {
    window.sessionStorage.removeItem("token");
    navigate("/game");
  };

  const showScoreboard = () => {
    navigate("/scoreboard");
  };

  return (
    <div>
      <h1>Main Menu</h1>
      <div>
        <ul>
          <li>
            Username: <input type="text" onChange={(e) => setNickname(e.target.value)}></input>
          </li>
          <li className="menu-option" onClick={startGame}>
            Start game
          </li>
          {/* <li className="menu-option">Settings</li> */}
          <li className="menu-option" onClick={showScoreboard}>
            Scoreboard
          </li>
        </ul>
      </div>
    </div>
  );
};

export default Menu;
