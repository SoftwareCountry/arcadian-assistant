import React from 'react';
import { ActivityIndicator, View } from 'react-native';
import { searchViewStyles as styles } from './search/search-view-styles';
import Style from '../layout/style';

//============================================================================
export class LoadingView extends React.Component {
    public render() {
        return (
            <View style={styles.loadingContainer}>
                <ActivityIndicator size={'large'} color={Style.color.base}/>
            </View>
        );
    }
}
