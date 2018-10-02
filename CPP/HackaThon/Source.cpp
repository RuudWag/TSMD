#include <iterator>
#include <iostream>
#include <fstream>
#include <sstream>
#include <vector>
#include <string>
#include <cmath>
#include <algorithm>

#include "CSVReader.h"
#include <set>
#include <iosfwd>
#include <iomanip>

class Coordinate
{
public:
	int x;
	int y;

	Coordinate( const int i_x, const int i_y ) :
		x( i_x ),
		y( i_y )
	{
	}
};

struct Node
{
	size_t m_prev;
	size_t m_next;
	bool m_inSolution;
};

std::vector<Coordinate> GetCoordinates( std::string i_fileName )
{
	std::ifstream       file( i_fileName );
	std::vector<Coordinate> coordinates;
	CSVRow row;
	while ( file >> row )
	{
		coordinates.emplace_back( row[ 1 ], row[ 2 ] );
	}
	return coordinates;
}

std::vector<std::vector<double>> GetDistanceMatrix( const std::vector<Coordinate>& i_coordinates )
{
	std::vector<std::vector<double>> distanceMatrix( i_coordinates.size() );
	for ( size_t i = 0; i < i_coordinates.size(); i++ )
	{
		distanceMatrix[ i ].resize( i_coordinates.size() );
		for ( size_t j = 0; j < i_coordinates.size(); j++ )
		{
			distanceMatrix[ i ][ j ] = std::sqrt( ::pow( i_coordinates[ i ].x - i_coordinates[ j ].x, 2 ) + std::pow( i_coordinates[ i ].y - i_coordinates[ j ].y, 2 ) );
		}
	}
	return distanceMatrix;

}

bool CheckSolution( std::vector<Node>& io_solution1, std::vector<Node>& io_solution2 )
{
	std::set<size_t> nodesVisited1;
	bool feasible = true;
	size_t currentNode = 0;
	do
	{
		if ( nodesVisited1.count( currentNode )  != 0 )
		{
			std::cout << "not feasible: Node " << currentNode << "is visited twice in solution 1" << std::endl;
			return false;
		}
		nodesVisited1.insert( currentNode );

		size_t nextNode = io_solution1[ currentNode ].m_next;
		if ( io_solution2[ currentNode ].m_prev == nextNode || io_solution2[ currentNode ].m_next == nextNode )
		{
			std::cout << "not feasible: Arc from node " << currentNode << " to node " << nextNode << " is in both solutions" << std::endl;
			feasible = false;
		}
		currentNode = io_solution1[ currentNode ].m_next;
	} while ( currentNode != 0 );

	if ( nodesVisited1.size() != io_solution1.size() )
	{
		std::cout << "not feasible: The following nodes are not visited in solution 1: " << std::endl;
		for ( size_t node = 0; node < io_solution1.size(); node++ )
		{
			if ( nodesVisited1.count( node ) == 0 )
			{
				std::cout << node << ", " << std::endl;
			}
		}
		feasible = false;
	}
	std::set<size_t> nodesVisited2;
	currentNode = 0;
	do
	{
		if ( nodesVisited2.count( currentNode ) != 0 )
		{
			std::cout << "not feasible: Node " << currentNode << "is visited twice in solution 2" << std::endl;
			return false;

		}
		nodesVisited2.insert( currentNode );
		currentNode = io_solution2[ currentNode ].m_next;
	} while ( currentNode != 0 );

	if ( nodesVisited2.size() != io_solution2.size() )
	{
		std::cout << "not feasible: The following nodes are not visited in solution 2: ";
		for ( size_t node = 0; node < io_solution2.size(); node++ )
		{
			if ( nodesVisited2.count( node ) == 0 )
			{
				std::cout << node << ", " << std::endl;
			}
		}
		feasible = false;
	}

	return feasible;
}

double GetDistanceSolution( std::vector<Node>& io_solution1, const std::vector<std::vector<double>>& i_distanceMatrix )
{
	double distance = 0.0;
	size_t currentNode = 0;
	do 
	{
		size_t nextnode = io_solution1[ currentNode ].m_next;
		distance += i_distanceMatrix[ currentNode ][ nextnode ];
		currentNode = nextnode;
	} while ( currentNode !=0);
	return distance;
}

void InsertNodeAt( std::vector<Node>& io_solution, const size_t i_insertAfterNode, const size_t i_insertNode )
{
	if ( io_solution[i_insertAfterNode].m_inSolution && !io_solution[ i_insertNode ].m_inSolution )
	{
		size_t NodeAfterInstertedNode = io_solution[ i_insertAfterNode ].m_next;
		io_solution[ i_insertAfterNode ].m_next = i_insertNode;
		io_solution[ NodeAfterInstertedNode ].m_prev = i_insertNode;

		io_solution[ i_insertNode ].m_prev = i_insertAfterNode;
		io_solution[ i_insertNode ].m_next = NodeAfterInstertedNode;

		io_solution[ i_insertNode ].m_inSolution = true;
	}
	else
	{
		if ( !io_solution[ i_insertAfterNode ].m_inSolution )
		{
			std::cout << "tried to insert after node " << i_insertAfterNode  << " ,but it is not in the solution " << std::endl;
		}
		else
		{
			std::cout << "tried to insert node " << i_insertNode << " ,but it is already in the solution " << std::endl;
		}
	}
}

void RemoveNode( std::vector<Node>& io_solution, const size_t i_removeNode )
{
	if ( io_solution[ i_removeNode ].m_inSolution )
	{
		size_t prevNode = io_solution[ i_removeNode ].m_prev;
		size_t nextNode = io_solution[ i_removeNode ].m_next;
		io_solution[ prevNode ].m_next = nextNode;
		io_solution[ nextNode ].m_prev = prevNode;
		io_solution[ i_removeNode ].m_inSolution = false;
	}
	else
	{
		std::cout << "tried to remove node " << i_removeNode << " ,but it is not in the solution " << std::endl;
	}
}

void Algorithm( std::vector<Node>& io_solution1, std::vector<Node>& io_solution2, const std::vector<std::vector<double>>& i_distanceMatrix )
{
	//initial random insert
	size_t insertAfterNode = 0;
	std::vector<size_t> nodesToInsert;
	for ( size_t indexNode = 1; indexNode < io_solution1.size(); indexNode++ )
	{
		nodesToInsert.push_back( indexNode );
	}
	//std::random_shuffle( nodesToInsert.begin(), nodesToInsert.end() );
	for ( auto node : nodesToInsert )
	{
		InsertNodeAt( io_solution1, insertAfterNode, node );
		insertAfterNode = node;
	}
	//std::random_shuffle( nodesToInsert.begin(), nodesToInsert.end() );
	insertAfterNode = 0;
	size_t middlePoint = static_cast<size_t>( std::ceil( double( nodesToInsert.size() - 1 ) / 2.0 ) );
	size_t left = 0;
	size_t right = nodesToInsert.size()-1;
	InsertNodeAt( io_solution2, insertAfterNode, nodesToInsert[middlePoint] );
	insertAfterNode = nodesToInsert[ middlePoint ];
	bool insertLeft = true;

	while ( !(left == middlePoint && right == middlePoint) )
	{
		size_t insertNode;
		if ( insertLeft )
		{
			insertNode = nodesToInsert[ left ];
			left++;
		}
		else
		{
			insertNode = nodesToInsert[ right ];
			
			right--;
		}
		InsertNodeAt( io_solution2, insertAfterNode, insertNode );
		insertAfterNode = insertNode;
		insertLeft = !insertLeft;
	}
}

void SaveSolutionToFile( const std::string& i_OutputFile, const std::vector<Node>& i_solution1, const std::vector<Node>& i_solution2 )
{
	std::ofstream outputFile( i_OutputFile );
	for ( int node = 0; node < i_solution1.size(); node++ )
	{
		outputFile << i_solution1[ node ].m_next << "," << i_solution2[ node ].m_next << std::endl;
	}
}

int main( int argc, char *argv[] )
{
	std::string InputFilePath( argv[ 1 ]);
	std::string OutputFilePath( argv[ 2 ] );
	std::vector<Coordinate> coordinates = GetCoordinates( InputFilePath );
	std::vector<std::vector<double>> distanceMatrix = GetDistanceMatrix( coordinates );
	std::vector<Node> solution1( coordinates.size() );
	solution1[ 0 ].m_inSolution = true;
	std::vector<Node> solution2( coordinates.size() );
	solution2[ 0 ].m_inSolution = true;
	std::cout << "Start Algorithm" << std::endl;
	Algorithm( solution1, solution2, distanceMatrix );
	std::cout << "Finished Algorithm" << std::endl;
	bool feasibleSolutions = CheckSolution( solution1, solution2 );
	if ( feasibleSolutions )
	{
		double distance1 = GetDistanceSolution( solution1, distanceMatrix );
		double distance2 = GetDistanceSolution( solution2, distanceMatrix );
		std::cout << "Distance solution 1: " << std::fixed << std::setprecision( 5 ) << distance1 << std::endl;
		std::cout << "Distance solution 2: " << std::fixed <<std::setprecision( 5 ) << distance2 << std::endl;
		std::cout << "Max Distance: " << std::fixed <<std::setprecision( 5 ) <<std::max( distance1, distance2 ) << std::endl;
		SaveSolutionToFile( OutputFilePath, solution1, solution2 );		
	}
	std::getchar();
}