#pragma once
/*session.h*/
#include "student.h"

using namespace std;

class Session
{
private:
	string SessionNumber;
	int Questions;
	int Answers;
	int CorrectAnswers;
	vector<Student> Students;

public:
	// default constructor
	Session(std::string ssnnNum, int ques, int ans, int correcAns, vector<Student> thisStudent)
		: SessionNumber(ssnnNum), Questions(ques), Answers(ans), CorrectAnswers(correcAns), Students(thisStudent)
	{
	}
	string getSessionNum() const
	{
		return SessionNumber;
	}
	int getQuestions() const
	{
		return Questions;
	}
	int getAnswers() const
	{
		return Answers;
	}
	int getCorrectAnswers() const
	{
		return CorrectAnswers;
	}
	auto  getStudentVector()
	{
		return Students;
	}
	auto getVectorSize()
	{
		return Students.size();
	}
	auto getBegin()
	{
		return Students.begin();
	}
	auto getEnd()
	{
		return Students.end();
	}
};


