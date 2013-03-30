#pragma once

#include "ImeUi.h"

namespace TouhouSpring {
namespace Ime {

public delegate void CharMessageHandler(System::Char code);
public delegate void InputLangChangeHandler(System::String^ lang);

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

    event CharMessageHandler^ OnChar;
    event InputLangChangeHandler^ OnInputLangChange;

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
