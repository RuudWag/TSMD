import csv
import sys
import math

if len(sys.argv) != 3:
	print("Give Coordinates and solution. E.g. python ValiDator.py input1000.csv Output1000.csv")
	sys.exit(1)


def GetDistance(fromNode, toNode):
    return math.sqrt(math.pow(fromNode[0]-toNode[0],2)+math.pow(fromNode[1]-toNode[1],2))

coordinatesfilepath = sys.argv[1]
solutionfilepath = sys.argv[2]

NumberOfCoordinates = 0
coordinates=[]
with open(coordinatesfilepath, newline='') as csvfile:
    rowReader = csv.reader(csvfile, delimiter=',')
    for row in rowReader:        
        NumberOfCoordinates += 1
        coordinate = ( int(row[1]), int(row[2]))
        coordinates.append(coordinate)
        

print( "Check solution with %s nodes"%NumberOfCoordinates)
Solution1 = []
Solution2 = []

with open(solutionfilepath, newline='') as csvfile:
    rowReader = csv.reader(csvfile, delimiter=',')
    for row in rowReader: 
        Solution1.append( int(row[0]) )
        Solution2.append( int(row[1]) )

nodeVisitid1 = set([])
distanceSolution1 = 0
currentNode = 0
while (True):
    if currentNode in nodeVisitid1:
        print ( "not feasible: Node %s is visited twice in solution 1"%currentNode)
    nodeVisitid1.add(currentNode)
    nextNode = Solution1[currentNode]
    if Solution2[currentNode] == nextNode or Solution2[nextNode] == currentNode:
        print("not feasible: Arc from node %s to node %s is in both solutions"%(currentNode, nextNode) )
    
    distanceSolution1 += GetDistance(coordinates[currentNode],coordinates[nextNode])

    currentNode = nextNode
    if (currentNode == 0):
        break
if len(nodeVisitid1) != NumberOfCoordinates:
    print("not feasible: The following nodes are not visited in solution 1: ")
    for node in range(0,len(NumberOfCoordinates)):
        if node not in nodeVisitid1:
            print(node)


nodeVisitid2 = set([])

distanceSolution2 = 0

while (True):
    if currentNode in nodeVisitid2:
        print ( "not feasible: Node %s is visited twice in solution 2"%currentNode)
    nodeVisitid2.add(currentNode)
    nextNode = Solution2[currentNode]

    distanceSolution2 += GetDistance(coordinates[currentNode],coordinates[nextNode])

    currentNode = nextNode

    if (currentNode == 0):
        break
if len(nodeVisitid2) != NumberOfCoordinates:
    print("not feasible: The following nodes are not visited in solution 2: ")
    for node in range(0,len(NumberOfCoordinates)):
        if node not in nodeVisitid2:
            print(node)

print ( "Check Solution: feasible" )
print("Distance Solution 1 %s"%distanceSolution1)
print("Distance Solution 2 %s"%distanceSolution2)
print("Distance Solution %s"%max(distanceSolution1, distanceSolution2))