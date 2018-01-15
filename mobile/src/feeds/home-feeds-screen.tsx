import React from 'react';
import { FlatList, Text, View} from 'react-native';
import { TopNavBar } from '../navigation/top-nav-bar';

const navBar =  new TopNavBar('Feeds');
const dataList = [
{key: 'First Feed'},
{key: 'Second Feed'},
{key: 'Third Feed'},
{key: 'Fourth Feed'},
{key: 'Fifth Feed'},
{key: 'Sixth Feed'},
{key: 'Seventh Feed'},
{key: 'Ten'}
];


export class HomeFeedsScreen extends React.Component {
    public static navigationOptions = navBar.configurate();

    public render() {
        return (
            <View>
                <FlatList data = {dataList} renderItem ={({item}) => <Text style={{padding: 10, fontSize: 18, height: 81, textAlign: 'center'}}>{item.key}</Text>} />
            </View>
        );
    }
}