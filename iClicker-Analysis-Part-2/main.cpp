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
#include "session.h"

using namespace std;


//======================================================================
void Answered(vector<Session> sessions)
{
	for (auto K : sessions)
	{
		cout << "  \"" << K.getSessionNum() << "\": " << (((double)K.getAnswers() / ((double)
			K.getVectorSize() * K.getQuestions())) * 100.0) << "% (" << K.getQuestions() <<
			" questions, " << K.getVectorSize() << " clickers, " << K.getAnswers() << " answers)" << endl;
	}
}


//======================================================================
void Correctly(vector<Session> sessions)
{
	for (auto K : sessions)
	{
		cout << "  \"" << K.getSessionNum() << "\": " << (((double)K.getCorrectAnswers() / ((double)
			K.getVectorSize() * K.getQuestions())) * 100.0) << "% (" << K.getQuestions() << " questions, "
			<< K.getVectorSize() << " clickers, " << K.getCorrectAnswers() << " correct answers)" << endl;
	}
}


//======================================================================
void ClickerIDFound(Session S, Student I, string cmd)
{
	auto it = find_if(S.getBegin(), S.getEnd(),
		[&](const Student& st)
	{
		if (st.getClickerID() == cmd)
			return true;
		else
			return false;
	});

	if (it != S.getEnd())
	{
		// clicker id found 
		cout << " \"" << S.getSessionNum() << "\": " << it->answered()
			<< " out of " << S.getQuestions() << " answered, " << ((double)
				it->correct() / S.getQuestions()) * 100.0 << "% correctly" << endl;
	}
}
//======================================================================
int main()
{
	string filename, line, clickerID, key, noneCorrecID, studResponse,
		halfCorrecID, correcResponse, extractSession, extractClicker, cmd;
	int qnCount = 0, answerdAll = 0, ssnnCount = 0;
	double percentage = 0.0;

	// Input data into a vector of sessions
	vector<Session> sessions;

	cout << "**Starting**" << endl;

	ifstream txtfile("files.txt");
	string txtline;

	while (getline(txtfile, txtline))
	{
		cout << ">>Parsing '" << txtline << "'..." << endl;

		ifstream xmlfile(txtline);
		string xmlline;

		while (getline(xmlfile, xmlline))
		{
			if (xmlline.find("<ssn") == 0)
				break;
		}

		auto position = xmlline.find("ssnn");
		position += 6;

		if (position != string::npos)
		{
			ssnnCount++;
			auto session = xmlline.find("\"", position);
			extractSession = xmlline.substr(position, session - position);
		}

		int questionCounter = 0;
		int clickerCounter = 0;
		int answerCounter = 0;
		int correctAnsCounter = 0;
		vector<Student> allStudent;
		while (getline(xmlfile, xmlline))
		{
			//===================ANSWER KEY==========================
			auto position = xmlline.find("cans");

			if (position != string::npos)
			{
				key = xmlline.substr(position + 6, 9);
				auto multi = key.find("\"");
				correcResponse = key.substr(0, multi);
			}
			//=====================QUESTIONS========================
			// Look for qn index
			auto question = xmlline.find("qn");

			if (question != string::npos)
			{
				// Increment whenever another qn is found
				qnCount++;
				questionCounter++;
			}
			if (xmlline.find("  <v") == 0)
			{
				auto clicker = xmlline.find("id");
				extractClicker = xmlline.substr(clicker + 5, 8);
				if (clicker != string::npos)
					clickerCounter++;

				auto answered = xmlline.find(" ans=");
				studResponse = xmlline.substr(answered + 6, 1);

				if (answered != string::npos&&studResponse.find("\""))
					answerCounter++;

				auto idcount = find_if(allStudent.begin(), allStudent.end(),
					[&](Student& s)
				{
					if (s.getClickerID() == extractClicker)
						return true;
					else
						return false;
				});
				if (idcount == allStudent.end())  // empty 
				{
					//=======================ANSWERED RIGHT==========================
					if (correcResponse.find(studResponse) != string::npos)
					{
						Student S(extractClicker, 1, 1);
						allStudent.push_back(S);
						correctAnsCounter++;
					}
					//=======================ANSWERED WRONG==========================
					else if (correcResponse.find(studResponse) == string::npos && studResponse.find("\""))
					{
						Student S(extractClicker, 1, 0);
						allStudent.push_back(S);
					}
					//=======================DID NOT ANSWER==========================
					else
					{
						Student S(extractClicker, 0, 0);
						allStudent.push_back(S);
					}
				}
				else
				{
					if (correcResponse.find(studResponse) != string::npos)
					{
						idcount->incrimentQuestionsAns();
						idcount->incrimentCorrectAns();
						correctAnsCounter++;
					}
					else if (correcResponse.find(studResponse) == string::npos && studResponse.find("\""))
						idcount->incrimentQuestionsAns();
				}
			}
		}
		Session K(extractSession, questionCounter, answerCounter, correctAnsCounter, allStudent);
		sessions.push_back(K);
	}
	cout << endl << "**Class Analysis**" << endl;
	cout << ">>Total sessions: " << ssnnCount << endl;
	cout << ">>Total questions: " << qnCount << endl;
	//=======================ANSWERED==========================
	cout << ">>Answered:" << endl;
	Answered(sessions);
	//=======================CORRECTLY=========================
	cout << ">>Correctly:" << endl;
	Correctly(sessions);

	//=======================INPUT ID's========================
	cout << endl << "**Student Analysis**" << endl;
	while (cmd != "#")
	{
		cout << ">> Enter a clicker id (# to quit): ";
		cin >> cmd;

		if (cmd == "#")
			break;
		for (Session S : sessions)
		{
			for (Student I : S.getStudentVector())
			{
				ClickerIDFound(S, I, cmd);
				break;
			}
		}
		// error checking
		if (cmd.length() < 8 || cmd.length() > 8)
			cout << "** not found..." << endl;
	}
	cout << "**END**" << endl;
	return 0;
}
