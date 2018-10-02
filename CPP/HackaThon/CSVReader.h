#pragma once
#include <iterator>
#include <iostream>
#include <fstream>
#include <sstream>
#include <vector>
#include <string>

class CSVRow
{
public:
	int const& operator[]( std::size_t index ) const
	{
		return m_data[ index ];
	}
	std::size_t size() const
	{
		return m_data.size();
	}
	void readNextRow( std::istream& str )
	{
		std::string         line;
		std::getline( str, line );

		std::stringstream   lineStream( line );
		std::string         cell;

		m_data.clear();
		while ( std::getline( lineStream, cell, ',' ) )
		{
			m_data.push_back( std::stoi( cell ) );
		}
	}
private:
	std::vector<int>    m_data;
};

std::istream& operator>>( std::istream& str, CSVRow& data )
{
	data.readNextRow( str );
	return str;
}