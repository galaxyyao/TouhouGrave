#pragma once

namespace TouhouSpring {
namespace Ime {

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

private:
	bool m_initialized;
};

}
}
