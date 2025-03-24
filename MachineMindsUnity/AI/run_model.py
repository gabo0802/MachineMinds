from joblib import load
from sklearn.tree import DecisionTreeClassifier
import pandas as pd
import sys

def get_ai_model_value(x_data):
    dt = DecisionTreeClassifier()
    dt = load('filename.joblib')
    return dt.predict(x_data)

if __name__ == "__main__":
    print(sys.argv)

    x_value = pd.DataFrame()
    x_value['currentDifficulty'] = sys.argv[0]
    x_value['currentPlayerLives'] = sys.argv[1]
    x_value['levelsBeat'] = sys.argv[2]
    x_value['playerLifeTimer'] = sys.argv[3]
    x_value['totalEnemiesKilled'] = sys.argv[4]
    x_value['totalPoints'] = sys.argv[5]

    print(get_ai_model_value(x_value))