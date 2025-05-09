from joblib import load
from sklearn.tree import DecisionTreeClassifier
import pandas as pd
import sys
import os

def get_ai_model_value(x_data):
    dt = DecisionTreeClassifier()
    current_directory = os.getcwd()
    __location__ = os.path.realpath(os.path.join(os.getcwd(), os.path.dirname(__file__)))   
    os.chdir(__location__) # ensure that the working directory is where this script is located.
    
    #dt = load('difficultyaimodelweights.joblib1')
    dt = load('difficulty_ai_weights_with_data_preprocessing2.joblib') #load model weights
    
    os.chdir(current_directory) # go back to the original working directory

    return dt.predict(x_data)[0] #predict data

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

        print(get_ai_model_value(x_value))