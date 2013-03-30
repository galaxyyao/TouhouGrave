#pragma once

#include "ImeUi.h"

namespace TouhouSpring {
namespace Ime {

public delegate void KeyMessageHandler(System::Char code);
public delegate void InputLangChangeHandler(System::String^ lang);
public delegate void CompositionMessageHandler(System::String^ compositionString, int cursorPos);
public delegate void EndCompositionMessageHandler();

public ref class ImeContext
{
public:
    ImeContext(System::IntPtr windowHandle);
    ~ImeContext();

    property bool IsInitialized
    {
        bool get() { return m_initialized; }
    }

    property System::String^ IndicatorString
    {
        System::String^ get();
    }

    void BeginIme();
    void EndIme();

    event KeyMessageHandler^ OnChar;
    event KeyMessageHandler^ OnKeyDown;
    event KeyMessageHandler^ OnKeyUp;
    event InputLangChangeHandler^ OnInputLangChange;
    event CompositionMessageHandler^ OnComposition;
    event EndCompositionMessageHandler^ OnEndComposition;

private:
    delegate LRESULT WndProcDelegate(HWND, UINT, WPARAM, LPARAM);
    delegate void InputLangChangeDelegate();
    LRESULT WindowProcedure(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam);
    bool StaticMsgProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam);
    void ImeOnInputLangChange();

    static ImeContext^ s_instance;
    bool m_initialized;
    WndProcDelegate^ m_wndProc;
    WNDPROC m_oldWndProc;
    InputLangChangeDelegate^ m_imeOnInputLangChange;
};

}
}