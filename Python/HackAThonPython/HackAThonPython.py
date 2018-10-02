import sys
import csv 
import math

coordinatesfilepath = sys.argv[1]

class Coordinate(object):
    def __init__(self,x,y):
        self.x = x
        self.y = y

class Node(object):
    def __init__(self):
        self.next = 0
        self.prev = 0
        self.inSolution = False


def GetCoordinates():
    coordinates=[]
    with open(coordinatesfilepath, newline='') as csvfile:
        rowReader = csv.reader(csvfile, delimiter=',')
        for row in rowReader:
            coordinates.append( Coordinate( int(row[1]), int(row[2])  )  )
            
    return coordinates

def SaveSolutions( solution1, solution2 ):
    with open('outputfor%s'%coordinatesfilepath, 'w', newline='') as csvfile:
        solutionwriter = csv.writer(csvfile, delimiter=',')
        for i in range(0,len(solution1)):
            solutionwriter.writerow( [solution1[i].next, solution2[i].next] )


def GetDistanceMatrix(coordinates):
    distanceMatrix = [[0 for x in range(len(coordinates))] for y in range(len(coordinates))]
    for i in range(0,len(coordinates)):
        for j in range(0,len(coordinates)):
            distanceMatrix[i][j] = math.sqrt(math.pow(coordinates[i].x-coordinates[j].x,2) + math.pow(coordinates[i].y-coordinates[j].y,2))

    return distanceMatrix

def CheckSolution( Solution1, Solution2 ):
    nodeVisitid1 = set([])
    currentNode = 0
    feasible = True
    while (True):
        if currentNode in nodeVisitid1:
            print ( "not feasible: Node %s is visited twice in solution 1"%currentNode)
            feasible = False
        nodeVisitid1.add(currentNode)
        nextNode = Solution1[currentNode].next
        if Solution2[currentNode].next == nextNode or Solution2[nextNode].next == currentNode:
            print("not feasible: Arc from node %s to node %s is in both solutions"%(currentNode, nextNode) )
            feasible = False

        currentNode = nextNode
        if (currentNode == 0):
            break
    if len(nodeVisitid1) != len(Solution1):
        feasible = False
        print("not feasible: The following nodes are not visited in solution 1: ")
        for node in range(0,len(Solution1)):
            if node not in nodeVisitid1:
                print(node)


    nodeVisitid2 = set([])

    distanceSolution2 = 0

    while (True):
        if currentNode in nodeVisitid2:
            feasible = False
            print ( "not feasible: Node %s is visited twice in solution 2"%currentNode)
        nodeVisitid2.add(currentNode)
        nextNode = Solution2[currentNode].next

        currentNode = nextNode

        if (currentNode == 0):
            break
    if len(nodeVisitid2) != len(Solution2):
        feasible = False
        print("not feasible: The following nodes are not visited in solution 2: ")
        for node in range(0,len(Solution2)):
            if node not in nodeVisitid2:
                print(node)

    return feasible


def InsertNodeAt( solution, insertAfterNode, InsertNode):
    if solution[insertAfterNode].inSolution and not solution[InsertNode].inSolution:
        NodeAfterInstertedNode = solution[insertAfterNode].next
        solution[ insertAfterNode ].next = InsertNode;
        solution[ NodeAfterInstertedNode ].prev = InsertNode;

        solution[ InsertNode ].prev = insertAfterNode;
        solution[ InsertNode ].next = NodeAfterInstertedNode;

        solution[ InsertNode ].inSolution = True;
    else:
        if not io_solution[ i_insertAfterNode ].m_inSolution:	
            print("tried to insert after node %s, but it is not in the solution "%insertAfterNode)		
        else:
            print("tried to insert node %s, but it is already in the solution "%InsertNode)	

def RemoveNode( solution, removeNode ):
	if solution[ removeNode ].inSolution:
		prevNode = solution[ removeNode ].prev;
		nextNode = solution[ removeNode ].next;
		solution[ prevNode ].next = nextNode;
		solution[ nextNode ].prev = prevNode;
		solution[ removeNode ].inSolution = False;	
	else:
		print("tried to remove node %S, but it is not in the solution "%i_removeNode)



def Algorithm(solution1, solution2, distanceMatrix):
    insertAfterNode = 0
    nodesToInsert = []
    for indexNode in range(1,len(solution1)):
        nodesToInsert.append(indexNode)
    for node in nodesToInsert:
        InsertNodeAt( solution1, insertAfterNode, node)
        insertAfterNode = node
    insertAfterNode = 0
    middlePoint = math.ceil((len(nodesToInsert)-1)/2)
    left = 0
    right = len(nodesToInsert)-1
    InsertNodeAt( solution2, insertAfterNode, nodesToInsert[middlePoint] )
    insertAfterNode = nodesToInsert[middlePoint]
    insertLeft=True
    while ( not ( left == middlePoint and right == middlePoint) ):        
        if insertLeft:
            insertNode = nodesToInsert[left]
            left += 1
        else:
            insertNode = nodesToInsert[right]
            right -= 1
        InsertNodeAt(solution2, insertAfterNode, insertNode)
        insertAfterNode = insertNode
        insertLeft = not insertLeft

def main():
    coordinates = GetCoordinates()
    distanceMatrix = GetDistanceMatrix(coordinates)
    solution1 = []
    solution2 = []
    for i in range(0,len(coordinates)):
        solution1.append(Node())
        solution2.append(Node())
    solution1[0].inSolution = True
    solution2[0].inSolution = True
    Algorithm(solution1, solution2, distanceMatrix)
    feasible = CheckSolution(solution1,solution2)

    if feasible:
        SaveSolutions( solution1, solution2 )

    print("finished")





main()