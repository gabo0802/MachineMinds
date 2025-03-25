from joblib import load
from sklearn.tree import DecisionTreeClassifier
import pandas as pd
import sys

def get_ai_model_value(x_data):
    dt = DecisionTreeClassifier()
    dt = load('filename.joblib')
    return dt.predict(x_data)[0]

if __name__ == "__main__":
    #command syntax: 'python -W ignore run_model.py currentDifficulty currentPlayerLives levelsBeat totalEnemiesKilled totalPoints
    #command example: 'python -W ignore run_model.py 1 3 3 2 44.75 11 0'

    #print(sys.argv)    
    if len(sys.argv) < 6:
        print("missing arguements")
    else:
        x_value = pd.DataFrame()
        x_value['currentDifficulty'] = [sys.argv[1]]
        x_value['currentPlayerLives'] = [sys.argv[2]]
        x_value['levelsBeat'] = [sys.argv[3]]
        x_value['playerLifeTimer'] = [sys.argv[4]]
        x_value['totalEnemiesKilled'] = [sys.argv[5]]
        x_value['totalPoints'] = [sys.argv[6]]

        #print(x_value)
        print(get_ai_model_value(x_value))