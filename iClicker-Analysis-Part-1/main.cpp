/*main.cpp*/
//
// iClicker Analysis program in modern C++.
//
// Hamza Shahid
// U. of Illinois, Chicago
// CS 341: Fall 2017
// Project 01
//

#include <iostream>
#include <iomanip>
#include <fstream>
#include <string>
#include <sstream>
#include <vector>
#include <algorithm>
#include <numeric>
#include <functional>
#include <list>
#include <cstring>

#include "student.h"

using namespace std;


//======================================================================
// FileExists:
// Returns true if the file exists, false if not.
bool FileExists(string filename)
{
	ifstream file(filename);
	return file.good();
}


//======================================================================
void DateAndTime(string& filename, string& year, string& month,
	string& date, string& hour, string& minute)
{
	year = filename.substr(1, 2);
	month = filename.substr(3, 2);
	date = filename.substr(5, 2);
	hour = filename.substr(7, 2);
	minute = filename.substr(9, 2);
}


//======================================================================
void GetSessionName(size_t position, string extract, string line)
{
	position = line.find("ssnn");
	position += 6;

	if (position != string::npos)
	{
		auto session = line.find("\"", position);
		string extract = line.substr(position, session - position);
		cout << "Name: " << "\"" << extract << "\"" << endl;
	}
}


//======================================================================
int main()
{
	string filename, line, clickerID, key, noneCorrecID,
		studResponse, halfCorrecID, correcResponse, extract, extractClicker;
	int qnCount = 0;
	int answerdAll = 0;
	size_t position = 0;

	// L1709061256.xml

	// Input data into a vector of students
	vector<Student> students;

	cout << "**Starting**" << endl;
	cout << "**Filename?**" << endl;
	cin >> filename;
	cout << filename << endl;

	if (!FileExists(filename))
	{
		cout << "Error: '" << filename << "' not found, exiting..." << endl;
		return -1;
	}

	// Open file and analyze the clicker data:
	cout << "**Analysis**" << endl;

	// Obtaining date and time from filename
	string year, month, date, hour, minute;
	DateAndTime(filename, year, month, date, hour, minute);

	cout << "Date: " << month << "/" << date << "/" << year << endl;
	cout << "Time: " << hour << ":" << minute << endl;

	ifstream reOpenFile(filename);
	getline(reOpenFile, line);

	while (getline(reOpenFile, line))
	{
		if (line.find("<ssn") == 0)
			break;
	}
	//========================SESSION========================
	GetSessionName(position, extract, line);
	while (getline(reOpenFile, line))
	{
		//===================ANSWER KEY==========================
		auto position = line.find("cans");

		if (position != string::npos)
		{
			key = line.substr(position + 6, 9);
			auto multi = key.find("\"");
			correcResponse = key.substr(0, multi);
		}
		//===================QUESTIONS==========================
		// Look for qn index
		auto question = line.find("qn");

		if (question != string::npos)
		{
			// Increment whenever another qn is found
			qnCount++;
		}
		//===================STUDENT ANSWER=====================
		if (line.find("  <v") == 0)
		{
			auto clicker = line.find("id");
			extractClicker = line.substr(clicker + 5, 8);

			auto answered = line.find(" ans=");
			studResponse = line.substr(answered + 6, 1);
			//            cout << studResponse<<endl;
			//===================CLICKERS==========================
			auto idcount = find_if(students.begin(), students.end(),
				[&](Student& s)
			{
				if (s.getClickerID() == extractClicker)
					return true;
				else
					return false;
			});
			if (idcount == students.end())
			{
				if (correcResponse.find(studResponse) != string::npos)
				{
					Student S(extractClicker, 1, 1);
					students.push_back(S);
				}
				else if (correcResponse.find(studResponse) == string::npos && studResponse.find("\""))
				{
					Student S(extractClicker, 1, 0);
					students.push_back(S);
				}
				else
				{
					Student S(extractClicker, 0, 0);
					students.push_back(S);
				}
			}
			else
			{
				if (correcResponse.find(studResponse) != string::npos)
				{
					idcount->incrimentQuestionsAns();
					idcount->incrimentCorrectAns();
				}
				else if (correcResponse.find(studResponse) == string::npos && studResponse.find("\""))
					idcount->incrimentQuestionsAns();
			}
		}
	}

	auto half = (((studResponse.size() + 1)) / 2);
	//=======================ALL ANSWERED==========================
	int answeredAll = count_if(students.begin(), students.end(),
		[&](const Student s)
	{
		if (s.answered() == qnCount)
			return true;
		else
			return false;
	});
	//====================ATLEAST HALF ANSWERED===================
	auto answeredHalf = count_if(students.begin(), students.end(),
		[&](const Student s)
	{
		if (half < s.answered())
			return true;
		else
			return false;
	});
	//===================ATLEST ONE ANSWERED=====================
	int answeredOne = count_if(students.begin(), students.end(),
		[&](const Student& s)
	{
		if (s.answered() >= 1)
		{
			return true;
		}
		else
			return false;
	});
	//======================NONE ANSWERED========================
	int answeredNone = count_if(students.begin(), students.end(),
		[&](const Student s)
	{
		if (s.answered() < 1)
		{
			return true;
		}
		else
			return false;
	});
	//=======================ALL CORRECT=========================
	int allCorrect = count_if(students.begin(), students.end(),
		[&](const Student s)
	{
		if (s.correct() == qnCount)
			return true;
		else
			return false;
	});
	//====================ATLEAST HALF CORRECT===================
	auto halfCorrect = count_if(students.begin(), students.end(),
		[&](const Student s)
	{
		if (s.correct() >= qnCount / 2)
			return true;
		else
			return false;
	});
	//=======================ONE CORRECT=========================
	int oneCorrect = count_if(students.begin(), students.end(),
		[&](const Student& s)
	{
		if (s.correct() >= 1)
		{
			return true;
		}
		else
			return false;
	});
	//=======================NONE CORRECT========================
	int noneCorrect = count_if(students.begin(), students.end(),
		[&](const Student& s)
	{
		if (s.correct() < 1)
		{
			return true;
		}
		else
			return false;
	});

	//====================PRINT STATEMENTS=======================
	cout << "# questions: " << qnCount << endl;
	cout << "# clickers: " << students.size() << endl;
	cout << "# of students who answered: " << endl;
	cout << "  All questions: " << answeredAll << endl;
	cout << "  At least half: " << answeredHalf << endl;
	cout << "  At least one: " << answeredOne << endl;
	cout << "  None: " << answeredNone << endl;
	cout << "# of students who answered correctly: " << endl;
	cout << "  All questions: " << allCorrect << endl;
	cout << "  At least half: " << halfCorrect << endl;
	cout << "  At least one: " << oneCorrect << endl;
	cout << "  None: " << noneCorrect << endl;
	cout << "Students who answered < half: " << endl;

	for (Student& S : students)
	{
		if (S.answered() < ((qnCount + 1) / 2))
			cout << "  " << S.getClickerID() << endl;
	}
	cout << "Students with 0 correct: " << endl;
	for (Student& S : students)
	{
		if (S.correct() <= 0)
			cout << "  " << S.getClickerID() << endl;
	}

	cout << "**END**" << endl;
    int i;
    cin >> i;
	return 0;
}
