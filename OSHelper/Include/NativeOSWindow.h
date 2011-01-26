#pragma once

class NativeOSWindow : public wxFrame
{
public:
	typedef void (*DeleteDelegate)();
	typedef void (*SizedDelegate)();
	typedef void (*ClosedDelegate)();

	NativeOSWindow(String caption, int width, int height, DeleteDelegate deleteCB, SizedDelegate sizedCB, ClosedDelegate closedCB);

	virtual ~NativeOSWindow(void);

	wxWindow* getMainControl()
	{
		return mainControl;
	}

private:
	DeleteDelegate deleteCB;
	SizedDelegate sizedCB;
	ClosedDelegate closedCB;
	wxWindow* mainControl;

	void OnSize(wxEvent& event)
	{
		event.Skip();
		sizedCB();
	}

	void OnClose(wxEvent& event)
	{
		event.Skip();
		closedCB();
	}
};
