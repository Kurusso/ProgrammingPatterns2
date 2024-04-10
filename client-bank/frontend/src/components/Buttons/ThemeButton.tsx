import React, {useEffect, useState} from 'react';
import {isAuthenticated} from "../../api/auth";
import {Theme, userSettings} from "../../api/userSettings";


const THEME_COLORS = {
    light: { background: '#fff', color: '#000' },
    dark: { background: '#000', color: '#fff' }
};


const ThemeButton: React.FC = () => {
    const [theme, setTheme] = useState<Theme>('light');

    useEffect(() => {

        const fetchTheme = async () => {
            try {
                if (!isAuthenticated())
                    throw new Error("not authenticated")
                let theme:Theme = await userSettings.GetTheme();
                setTheme(theme);
            }
            catch (e ){
                console.log(e)
            }

        }

        fetchTheme();

    }, []);

    async function HandleThemeChange() {
        if (!isAuthenticated())
            throw new Error("not authenticated")
         await userSettings.ChangeTheme();

        setTheme(prevTheme => prevTheme === 'light' ? 'dark' : 'light');
    }

    return (
        <button
            onClick={HandleThemeChange}
            style={{
                backgroundColor: THEME_COLORS[theme].background,
                color: THEME_COLORS[theme].color,
            }}
        >
            Switch to {theme === 'light' ? 'Dark' : 'Light'} Theme
        </button>
    );
};

export default ThemeButton;
