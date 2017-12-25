import React from 'react';
import { FlatList, Text, View} from 'react-native';
import { TopNavBar } from '../topNavBar/top-nav-bar';

const navBar =  new TopNavBar('Feeds');
const dataList = [
{name: 'First Feed'},
{name: 'Second Feed'},
{name: 'Third Feed'},
{name: 'Fourth Feed'},
{name: 'Fifth Feed'},
{name: 'Sixth Feed'},
{name: 'Seventh Feed'},
{name: 'Ten'}
];


export class HomeFeedsScreen extends React.Component {
    public static navigationOptions = navBar.configurate();

    public render() {
        return (
            <View>
                <FlatList data = {dataList} renderItem ={({item}) => <Text style={{padding: 10, fontSize: 18, height: 81, textAlign: 'center'}}>{item.name}</Text>} />
            </View>
        );
    }
}