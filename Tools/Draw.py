import matplotlib.pyplot as plt
from matplotlib.collections import LineCollection
import numpy as np
import csv
import sys

if len(sys.argv) != 3:
	print("Give Coordinates and solution. E.g. python Draw.py 1000 Output1000.csv")
	sys.exit(1)

coordinatesfilepath = sys.argv[1]
solutionfilepath = sys.argv[2]

fig, ax = plt.subplots()
coordinates=[]
with open(coordinatesfilepath, newline='') as csvfile:
    rowReader = csv.reader(csvfile, delimiter=',')
    for row in rowReader:
        
        coordinate = ( int(row[1]), int(row[2]))
        coordinates.append(coordinate)
       
        plt.plot(coordinate[0], coordinate[1], marker='o', markersize = 1)   

linesSolution1 = []
linesSolution2 = []

with open(solutionfilepath, newline='') as csvfile:
    rowReader = csv.reader(csvfile, delimiter=',')
    rowNumber = 0
    for row in rowReader: 
        #linesSolution1.append( [coordinates[rowNumber], coordinates[int(row[0])]] )
        #linesSolution2.append( [coordinates[rowNumber], coordinates[int(row[1])]] )
        rowNumber += 1

lc = LineCollection(linesSolution1)
lc2 = LineCollection(linesSolution2, color = [(1, 0, 0, 1)])
ax.add_collection(lc)
ax.add_collection(lc2)


plt.show()