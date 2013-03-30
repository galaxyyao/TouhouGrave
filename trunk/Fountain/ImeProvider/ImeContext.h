#pragma once

#include "ImeUi.h"

namespace TouhouSpring {
namespace Ime {

public delegate void CharMessageHandler(System::Char code);

public ref class ImeContext
{
public:
    ImeContext(System::IntPtr windowHandle);
    ~ImeContext();

    property bool IsInitialized
    {
        bool get() { return m_initialized; }
    }

    void BeginIme();
    void EndIme();

    event CharMessageHandler^ OnChar;

private:
    delegate LRESULT WndProcDelegate(HWND, UINT, WPARAM, LPARAM);
    LRESULT WindowProcedure(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam);
    bool StaticMsgProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam);

    static ImeContext^ s_instance;
    bool m_initialized;
    WndProcDelegate^ m_wndProc;
    WNDPROC m_oldWndProc;
};

}
}
