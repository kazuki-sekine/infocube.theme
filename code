import pandas as pd 
import json 
import re 
 
JsonString = '''users = [ 
  { 
    "id": 1, 
    "name": "sato", 
    "logs": [ 
      {"time":1553101195196, "score":84}, 
      {"time":1552993677688, "score":43}, 
      {"time":1552901583366, "score":62}, 
      {"time":1552781080350, "score":95}, 
      {"time":1552689530078, "score":51} 
    ] 
  }, 
  { 
    "id": 2, 
    "name": "ito", 
    "logs": [ 
      {"time":1553101196196, "score":74}, 
      {"time":1552993678688, "score":33}, 
      {"time":1552901584366, "score":52}, 
      {"time":1552781081350, "score":85}, 
      {"time":1552689531078, "score":41} 
    ] 
  }, 
  { 
    "id":3, 
    "name": "suzuki", 
    "logs": [ 
      {"time":1553101197196, "score":64}, 
      {"time":1552993679688, "score":23}, 
      {"time":1552901585366, "score":42}, 
      {"time":1552781082350, "score":75}, 
      {"time":1552689532078, "score":31} 
    ] 
  } 
]''' 
 
JsonObject = json.loads(re.sub(r'^\s*users\s*=\s*', '', JsonString)) 
DataFrame  = pd.json_normalize(JsonObject, record_path=['logs'], meta=['id', 'name']) 
 
n     = 5 
floor = 1552993677688 
ceil  = 1553101195196 
 
# Question 1 
q1_result = \
  DataFrame.loc[DataFrame.groupby('id')['score'].idxmax(), :] \
  .sort_values(by='score', ascending=False).head(n) \
  .loc[:, ['id', 'name', 'time', 'score']] 
print("Q1A \n", q1_result, "\n") 
 
# Question 2 
q2_result = \
  DataFrame.loc[DataFrame.groupby('id')['time'].idxmax(), :] \
  .sort_values(by='time', ascending=False).head(n) \
  .loc[:, ['id', 'name', 'time', 'score']] 
print("Q2A \n", q2_result, "\n") 
 
# Question 3A 
q3_result = \
  DataFrame[(floor <= DataFrame['time']) & (DataFrame['time'] <= ceil)] \
  .groupby(['id', 'name'], as_index=False).mean() \
  .drop('time', axis=1) 
print("Q3A \n", q3_result, "\n") 
